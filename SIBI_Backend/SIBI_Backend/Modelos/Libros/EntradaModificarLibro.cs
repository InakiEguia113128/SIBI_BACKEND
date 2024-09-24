namespace SIBI_Backend.Modelos.Libros
{
    public class EntradaModificarLibro
    {
        public string codigoIsbn { get; set; } = null!;
        public string titulo { get; set; } = null!;
        public int cantidadEjemplares { get; set; }
        public string descripcion { get; set; } = null!;
        public string nombreAutor { get; set; } = null!;
        public string editorial { get; set; } = null!;
        public DateOnly fechaPublicacion { get; set; }
        public int? nroEdicion { get; set; }
        public int? nroVolumen { get; set; }
        public string codUbicacion { get; set; } = null!;
        public Guid idGenero { get; set; }
        public string? nGenero { get; set; }
        public int precioAlquiler { get; set; }
        public string imagenPortadaBase64 { get; set; } = null!;
    }
}