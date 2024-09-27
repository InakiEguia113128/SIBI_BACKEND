namespace SIBI_Backend.Modelos.Socios
{
    public class EntradaRegistrarSocio
    {
        public DateTime fechaNacimiento { get; set; } 
        public int numeroTelefono { get; set; } 
        public string calle { get; set; } = string.Empty; 
        public int altura { get; set; } 
        public Guid idSexo { get; set; } 
        public Guid idTipoDocumento { get; set; } 
        public long nroDocumento { get; set; } 
    }
}