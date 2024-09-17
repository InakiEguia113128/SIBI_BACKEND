namespace SIBI_Backend.Modelos.Usuarios
{
    public class EntradaModificarUsuario
    {
        public Guid idUsuario { get; set; }
        public string nombre {  get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string contrasenia { get; set; }
    }
}