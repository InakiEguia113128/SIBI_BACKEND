namespace SIBI_Backend.Modelos.Libros
{
    public class EntradaObtenerCatalogo
    {
        public string? titulo { get; set; }
        public string? autos { get; set; }
        public string? editorial { get; set; }
        public DateTime? fechaPublicacionDesde { get; set; }
        public DateTime? fechaPublicacionHasta { get; set; }
        public Guid? idGenero { get; set; }
        public string? nGenero { get; set; }
        public decimal? precioDesde { get; set; }
        public decimal? precioHasta { get; set; }
        public string? ISBN {  get; set; }
        public int devolver { get; set; }
        public int salta {  get; set; }
    }
}
