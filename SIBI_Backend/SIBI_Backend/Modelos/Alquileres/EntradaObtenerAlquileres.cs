namespace SIBI_Backend.Modelos.Alquileres
{
    public class EntradaObtenerAlquileres
    {
        public DateTime? fechaDesde { get; set; }
        public DateTime? fechaHasta { get; set; }
        public Guid? idEstadoAlquiler { get; set; }
        public Guid? idUsuario { get; set; }
        public Guid? idTipoDocumentoSocio { get; set; }
        public long? nroDocumentoSocio { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int salta { get; set; }
        public int devolver { get; set; }
    }
}
