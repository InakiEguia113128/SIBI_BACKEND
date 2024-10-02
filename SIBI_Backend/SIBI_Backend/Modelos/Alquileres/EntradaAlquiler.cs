namespace SIBI_Backend.Modelos.Alquileres
{
    public class EntradaAlquiler
    {
        public Guid idSocio {  get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int montoTotal { get; set; }
        public int? puntosCanjeados { get; set; }
        public List<EntradaDetalleAlquiler> detalleAlquiler { get; set; }
    }
}
