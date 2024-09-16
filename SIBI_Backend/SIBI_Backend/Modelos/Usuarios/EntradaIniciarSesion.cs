using System.ComponentModel.DataAnnotations;

namespace SIBI_Backend.Modelos.Usuarios
{
    public class EntradaIniciarSesion
    {
        [Required(ErrorMessage = "El email es requerido")]
        public string email { get; set; }

        [Required(ErrorMessage = "La contrseña es requerida")]
        public string contrasenia { get; set; }
    }
}