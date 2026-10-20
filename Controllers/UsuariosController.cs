
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindIt.API.Data;

namespace FindIt.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Listar usuarios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: Buscar usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }
    }
}
