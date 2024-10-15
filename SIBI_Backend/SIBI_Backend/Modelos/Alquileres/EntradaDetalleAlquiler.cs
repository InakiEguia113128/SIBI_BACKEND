namespace SIBI_Backend.Modelos.Alquileres
{
    public class EntradaDetalleAlquiler
    {
        public Guid idLibro {  get; set; }
        public decimal precioAlquiler { get; set; }
        public decimal subtotalDetalle {  get; set; }
    }
}
