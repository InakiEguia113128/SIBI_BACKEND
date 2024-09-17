using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SIBI_Backend.Data;

public partial class SibiDbContext : DbContext
{
    public SibiDbContext()
    {
    }

    public SibiDbContext(DbContextOptions<SibiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAlquilere> TAlquileres { get; set; }

    public virtual DbSet<TBarrio> TBarrios { get; set; }

    public virtual DbSet<TCiudade> TCiudades { get; set; }

    public virtual DbSet<TDetallesAlquiler> TDetallesAlquilers { get; set; }

    public virtual DbSet<TEstadosAlquiler> TEstadosAlquilers { get; set; }

    public virtual DbSet<TGenerosLibro> TGenerosLibros { get; set; }

    public virtual DbSet<TLibro> TLibros { get; set; }

    public virtual DbSet<TProvincia> TProvincias { get; set; }

    public virtual DbSet<TRole> TRoles { get; set; }

    public virtual DbSet<TRolesUsuario> TRolesUsuarios { get; set; }

    public virtual DbSet<TSocio> TSocios { get; set; }

    public virtual DbSet<TTiposDocumento> TTiposDocumentos { get; set; }

    public virtual DbSet<TTiposSexo> TTiposSexos { get; set; }

    public virtual DbSet<TUsuario> TUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Username=sibi_db;Password=sibi_db;Database=SIBI_DB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAlquilere>(entity =>
        {
            entity.HasKey(e => e.IdAlquiler).HasName("t_alquileres_pkey");

            entity.ToTable("t_alquileres");

            entity.Property(e => e.IdAlquiler)
                .ValueGeneratedNever()
                .HasColumnName("id_alquiler");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaDesde).HasColumnName("fecha_desde");
            entity.Property(e => e.FechaHasta).HasColumnName("fecha_hasta");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.IdEstadoAlquiler).HasColumnName("id_estado_alquiler");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.MontoTotal).HasColumnName("monto_total");
            entity.Property(e => e.PuntosCanjeados).HasColumnName("puntos_canjeados");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empleado");

            entity.HasOne(d => d.IdEstadoAlquilerNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdEstadoAlquiler)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_estado_alquiler");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_socio");
        });

        modelBuilder.Entity<TBarrio>(entity =>
        {
            entity.HasKey(e => e.IdBarrio).HasName("t_barrios_pkey");

            entity.ToTable("t_barrios");

            entity.Property(e => e.IdBarrio)
                .ValueGeneratedNever()
                .HasColumnName("id_barrio");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");

            entity.HasOne(d => d.IdCiudadNavigation).WithMany(p => p.TBarrios)
                .HasForeignKey(d => d.IdCiudad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ciudad");
        });

        modelBuilder.Entity<TCiudade>(entity =>
        {
            entity.HasKey(e => e.IdCiudad).HasName("t_ciudades_pkey");

            entity.ToTable("t_ciudades");

            entity.Property(e => e.IdCiudad)
                .ValueGeneratedNever()
                .HasColumnName("id_ciudad");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.IdProvincia).HasColumnName("id_provincia");

            entity.HasOne(d => d.IdProvinciaNavigation).WithMany(p => p.TCiudades)
                .HasForeignKey(d => d.IdProvincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_provincia");
        });

        modelBuilder.Entity<TDetallesAlquiler>(entity =>
        {
            entity.HasKey(e => e.IdDetalleAlquiler).HasName("t_detalles_alquiler_pkey");

            entity.ToTable("t_detalles_alquiler");

            entity.Property(e => e.IdDetalleAlquiler)
                .ValueGeneratedNever()
                .HasColumnName("id_detalle_alquiler");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.IdAlquiler).HasColumnName("id_alquiler");
            entity.Property(e => e.IdLibro).HasColumnName("id_libro");
            entity.Property(e => e.PrecioAlquiler).HasColumnName("precio_alquiler");

            entity.HasOne(d => d.IdAlquilerNavigation).WithMany(p => p.TDetallesAlquilers)
                .HasForeignKey(d => d.IdAlquiler)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alquiler");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.TDetallesAlquilers)
                .HasForeignKey(d => d.IdLibro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_libro");
        });

        modelBuilder.Entity<TEstadosAlquiler>(entity =>
        {
            entity.HasKey(e => e.IdEstadoAlquiler).HasName("t_estados_alquiler_pkey");

            entity.ToTable("t_estados_alquiler");

            entity.Property(e => e.IdEstadoAlquiler)
                .ValueGeneratedNever()
                .HasColumnName("id_estado_alquiler");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TGenerosLibro>(entity =>
        {
            entity.HasKey(e => e.IdGeneroLibro).HasName("t_generos_libro_pkey");

            entity.ToTable("t_generos_libro");

            entity.Property(e => e.IdGeneroLibro)
                .ValueGeneratedNever()
                .HasColumnName("id_genero_libro");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TLibro>(entity =>
        {
            entity.HasKey(e => e.IdLibro).HasName("t_libros_pkey");

            entity.ToTable("t_libros");

            entity.Property(e => e.IdLibro)
                .ValueGeneratedNever()
                .HasColumnName("id_libro");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CantidadEjemplares).HasColumnName("cantidad_ejemplares");
            entity.Property(e => e.CodUbicacion)
                .HasColumnType("character varying")
                .HasColumnName("cod_ubicacion");
            entity.Property(e => e.CodigoIsbn)
                .HasColumnType("character varying")
                .HasColumnName("codigo_isbn");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.Editorial)
                .HasColumnType("character varying")
                .HasColumnName("editorial");
            entity.Property(e => e.FechaPublicacion).HasColumnName("fecha_publicacion");
            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.ImagenPortadaBase64).HasColumnName("imagen_portada_base64");
            entity.Property(e => e.NGenero)
                .HasColumnType("character varying")
                .HasColumnName("n_genero");
            entity.Property(e => e.NombreAutor)
                .HasColumnType("character varying")
                .HasColumnName("nombre_autor");
            entity.Property(e => e.NroEdicion).HasColumnName("nro_edicion");
            entity.Property(e => e.NroVolumen).HasColumnName("nro_volumen");
            entity.Property(e => e.PrecioAlquiler).HasColumnName("precio_alquiler");
            entity.Property(e => e.Titulo)
                .HasColumnType("character varying")
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdGeneroNavigation).WithMany(p => p.TLibros)
                .HasForeignKey(d => d.IdGenero)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_genero");
        });

        modelBuilder.Entity<TProvincia>(entity =>
        {
            entity.HasKey(e => e.IdProvincia).HasName("t_provincias_pkey");

            entity.ToTable("t_provincias");

            entity.Property(e => e.IdProvincia)
                .ValueGeneratedNever()
                .HasColumnName("id_provincia");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TRole>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("t_roles_pkey");

            entity.ToTable("t_roles");

            entity.Property(e => e.IdRol)
                .ValueGeneratedNever()
                .HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TRolesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRolUsuario).HasName("t_roles_usuario_pkey");

            entity.ToTable("t_roles_usuario");

            entity.Property(e => e.IdRolUsuario)
                .ValueGeneratedNever()
                .HasColumnName("id_rol_usuario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TRolesUsuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rol");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TRolesUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario");
        });

        modelBuilder.Entity<TSocio>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("t_socios_pkey");

            entity.ToTable("t_socios");

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Altura).HasColumnName("altura");
            entity.Property(e => e.Calle)
                .HasColumnType("character varying")
                .HasColumnName("calle");
            entity.Property(e => e.CalleDomicilio)
                .HasColumnType("character varying")
                .HasColumnName("calle_domicilio");
            entity.Property(e => e.CodigoPostal).HasColumnName("codigo_postal");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdBarrio).HasColumnName("id_barrio");
            entity.Property(e => e.IdSexo).HasColumnName("id_sexo");
            entity.Property(e => e.IdTipoDocumento).HasColumnName("id_tipo_documento");
            entity.Property(e => e.NroDocumento).HasColumnName("nro_documento");
            entity.Property(e => e.NumeroTelefono).HasColumnName("numero_telefono");
            entity.Property(e => e.PuntosAcumulados).HasColumnName("puntos_acumulados");

            entity.HasOne(d => d.IdBarrioNavigation).WithMany(p => p.TSocios)
                .HasForeignKey(d => d.IdBarrio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_barrio");

            entity.HasOne(d => d.IdSexoNavigation).WithMany(p => p.TSocios)
                .HasForeignKey(d => d.IdSexo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sexo");

            entity.HasOne(d => d.IdTipoDocumentoNavigation).WithMany(p => p.TSocios)
                .HasForeignKey(d => d.IdTipoDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tipo_documento");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.TSocio)
                .HasForeignKey<TSocio>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_socio");
        });

        modelBuilder.Entity<TTiposDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento).HasName("t_tipos_documento_pkey");

            entity.ToTable("t_tipos_documento");

            entity.Property(e => e.IdTipoDocumento)
                .ValueGeneratedNever()
                .HasColumnName("id_tipo_documento");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TTiposSexo>(entity =>
        {
            entity.HasKey(e => e.IdTipoSexo).HasName("t_tipos_sexo_pkey");

            entity.ToTable("t_tipos_sexo");

            entity.Property(e => e.IdTipoSexo)
                .ValueGeneratedNever()
                .HasColumnName("id_tipo_sexo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<TUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("t_usuarios_pkey");

            entity.ToTable("t_usuarios");

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasColumnType("character varying")
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.HashContraseña).HasColumnName("hash_contraseña");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
