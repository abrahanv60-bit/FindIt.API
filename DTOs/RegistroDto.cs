
using System.ComponentModel.DataAnnotations;

namespace FindIt.API.DTOs
{
    public class RegistroDto
    {
        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = "";
    }
}
