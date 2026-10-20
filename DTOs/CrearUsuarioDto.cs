
using System.ComponentModel.DataAnnotations;

namespace FindIt.API.DTOs
{
    public class CrearUsuarioDto
    {
        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
