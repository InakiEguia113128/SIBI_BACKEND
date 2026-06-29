# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

SIBI is the backend for a library management system (Sistema Integral de Biblioteca). It is an ASP.NET Core 7 Web API over a PostgreSQL database, covering users/auth, members (socios), book catalog, rentals (alquileres), MercadoPago payments, email notifications, and reports. The codebase and domain language are in Spanish — follow that convention (e.g. `EntradaRegistrarUsuario`, `ServicioAlquiler`, table prefix `t_`).

The solution lives two levels deep: `SIBI_Backend/SIBI_Backend.sln` → `SIBI_Backend/SIBI_Backend/SIBI_Backend.csproj`.

## Commands

Run from the directory containing the `.csproj` (`SIBI_Backend/SIBI_Backend/`):

```bash
dotnet build                        # build
dotnet run                          # run the API (Swagger at /swagger in Development)
dotnet ef migrations add <Name>     # only if switching to migrations (see note below)
```

There is **no test project** in this repo.

### Database is scaffolded (database-first)

The EF Core model is generated from an existing PostgreSQL schema, not created via migrations. To regenerate the `Data/` entities and `SibiDbContext` after a schema change, run (command is also kept as a comment in `appsettings.json`):

```bash
dotnet ef dbcontext scaffold "Server=localhost;Username=sibi_db;Password=sibi_db;Database=SIBI_DB" Npgsql.EntityFrameworkCore.PostgreSQL -o Data --force
```

`--force` overwrites everything in `Data/`. Do not hand-edit generated entity files expecting changes to survive a re-scaffold; change the database schema and re-scaffold instead.

## Configuration

`appsettings.json` is **gitignored** (it holds real secrets) and must exist locally for the app to run. Required keys:

- `ConnectionStrings:Psql` — PostgreSQL connection string.
- `AppSettings:Token` — symmetric signing key for JWT.
- `AppSettings:Expires` — token lifetime in **days** (parsed as a double).
- `SendGrid:ApiKey` — SendGrid API key for email.

Note: `SibiDbContext.OnConfiguring` has a hardcoded fallback connection string; the live connection comes from `Program.cs` via `UseNpgsql(GetConnectionString("Psql"))`.

## Architecture

Request flow is a thin **Controller → Service → EF Core DbContext** stack. Controllers do no business logic; they call a service and translate the result into HTTP responses.

- **Controllers/** — one per domain (`Usuarios`, `Socios`, `Libros`, `Alquileres`, `MercadoPago`, `Reportes`). Routes are `api/[controller]` with Spanish kebab-case action routes (e.g. `[HttpPost("registrar-usuario")]`). All controllers except `Usuarios` are `[Authorize]` (login/registration must stay anonymous).
- **Servicios/** — business logic. Each domain has an `IServicioX` interface + `ServicioX` implementation, registered as **scoped** in `Program.cs`. Services take `SibiDbContext`, `IConfiguration`, and other services via constructor injection.
- **Modelos/** — request DTOs, conventionally prefixed `Entrada*` (input models), grouped by domain.
- **Data/** — scaffolded EF entities (`T*` classes, e.g. `TUsuario`, `TAlquilere`) and `SibiDbContext`. Navigation properties follow the `*Navigation` convention (e.g. `IdSocioNavigation`).
- **Comunes/** — shared constants: `RolesConstante` (role GUIDs) and `EstadosAlquilerContante` (rental-state GUIDs). Domain state machines key off these GUIDs rather than enums — look here before hardcoding any state/role ID.

### Service return convention

Every service method returns `Task<ResultadoBase>` (`Modelos/ResultadoBase.cs`): `{ Mensaje, Ok, Error, CodigoEstado, Resultado }`, where `Resultado` is `dynamic` (services build anonymous objects for the payload). Controllers inspect `respuesta.Ok` and return `Ok(...)` or `BadRequest(...)`. Service methods wrap their body in `try/catch` and set `Ok=false` + an error message on failure rather than throwing. Match this pattern when adding endpoints.

### Auth

JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`). `ServicioUsuario.IniciarSesion` issues the token with claims including roles (comma-joined role descriptions). Passwords are stored as a SHA-256 hash (`HashContraseña` byte column); compare by hashing the input, not by reversing. CORS is fully open (`AllowAnyOrigin/Header/Method`).

### Notifications (background service)

`ServicioNotificaciones` is registered **both** as a scoped service (for one-off emails like welcome/return confirmations) **and** as an `IHostedService` / `BackgroundService` via `AddHostedService`. The background loop runs once per day (`Task.Delay(TimeSpan.FromDays(1))`), scanning rentals to email members about upcoming and overdue returns, and applies point penalties (`PuntosAcumulados`) on overdue items. Because it's a singleton hosted service, it resolves `SibiDbContext` through `IServiceProvider.CreateScope()` per run — never inject a scoped DbContext directly into it. Emails use SendGrid dynamic templates (template IDs are hardcoded in this file).

## OpenSpec

This repo uses OpenSpec (spec-driven workflow) — see `openspec/` and the `openspec-*` / `opsx:*` skills. Active change proposals live in `openspec/changes/`, established specs in `openspec/specs/`.
