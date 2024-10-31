namespace SIBI_Backend.Modelos.Reportes
{
    public class EntradaReporteLibrosAlquilados
    {
        public string? titulo { get; set; }
        public string? autor { get; set; }
        public string? editorial { get; set; }
        public DateTime? fechaPublicacionDesde { get; set; }
        public DateTime? fechaPublicacionHasta { get; set; }
        public Guid? idGenero { get; set; }
        public string? nGenero { get; set; }
        public int devolver { get; set; }
        public int salta { get; set; }
    }
}