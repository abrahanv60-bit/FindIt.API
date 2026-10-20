
namespace FindIt.API.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        public int ObjetoId { get; set; }
        public int UsuarioId { get; set; }

        public string Mensaje { get; set; } = "";
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaSolicitud { get; set; }
            = DateTime.UtcNow;
    }
}
