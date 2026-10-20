
namespace FindIt.API.Models
{
    public class Objeto
    {
        public int? UsuarioId { get; set; }
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Ubicacion { get; set; } = "";
        public string Estado { get; set; } = "Disponible";
        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;
    }
}
