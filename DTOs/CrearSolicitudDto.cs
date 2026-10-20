
using System.ComponentModel.DataAnnotations;

namespace FindIt.API.DTOs
{
    public class CrearSolicitudDto
    {
        [Range(1, int.MaxValue)]
        public int ObjetoId { get; set; }

        [Required]
        public string Mensaje { get; set; } = "";
    }
}
