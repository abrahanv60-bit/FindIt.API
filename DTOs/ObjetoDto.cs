
using System.ComponentModel.DataAnnotations;

namespace FindIt.API.DTOs
{
    public class ObjetoDto
    {
        [Required]
        public string Titulo { get; set; } = "";

        [Required]
        public string Descripcion { get; set; } = "";

        [Required]
        public string Tipo { get; set; } = "";

        [Required]
        public string Ubicacion { get; set; } = "";

        public string Estado { get; set; } = "Disponible";
    }
}
