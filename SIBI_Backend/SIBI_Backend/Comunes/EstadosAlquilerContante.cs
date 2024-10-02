namespace SIBI_Backend.Comunes
{
    public class EstadosAlquilerContante
    {
        public static readonly Guid Pagado = new Guid("804259b3-b2cf-4197-95ce-6531c59321d5");
        public static readonly Guid Pendiente_devolucion = new Guid("0b713be9-da40-49c7-b20c-7247e5e7aafc");
        public static readonly Guid En_curso = new Guid("add4c9a1-b015-4ffb-b338-cdd818bc72be");
        public static readonly Guid Devolucion_vencida = new Guid("f044d8fa-e842-4bc8-a7a3-e7d6b1fe4425");
        public static readonly Guid Devuelto = new Guid("0781e3cf-0e6a-4799-bc73-4022796137a4");
    }
}
