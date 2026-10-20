
using System.ComponentModel.DataAnnotations;

namespace FindIt.API.DTOs
{
    public class CambiarEstadoDto
    {
        [Required]
        public string Estado { get; set; } = "";
    }
}
