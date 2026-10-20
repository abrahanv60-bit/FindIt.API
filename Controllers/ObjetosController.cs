
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FindIt.API.Data;
using FindIt.API.Models;
using FindIt.API.DTOs;

namespace FindIt.API.Controllers
{
    [ApiController]
    [Route("api/objetos")]
    public class ObjetosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ObjetosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Todos pueden ver los objetos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var objetos = await _context.Objetos.ToListAsync();
            return Ok(objetos);
        }

        // GET: Buscar objeto por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var objeto = await _context.Objetos.FindAsync(id);

            if (objeto == null)
                return NotFound();

            return Ok(objeto);
        }

        // POST: Solo usuarios autenticados
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(ObjetoDto datos)
        {
            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            if (datos.Tipo != "Perdido" &&
                datos.Tipo != "Encontrado")
                return BadRequest("Tipo no válido");

            var objeto = new Objeto
            {
                Titulo = datos.Titulo,
                Descripcion = datos.Descripcion,
                Tipo = datos.Tipo,
                Ubicacion = datos.Ubicacion,
                Estado = "Disponible",
                UsuarioId = usuarioId,
                FechaPublicacion = DateTime.UtcNow
            };

            _context.Objetos.Add(objeto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = objeto.Id },
                objeto
            );
        }

        // PUT: Solo el propietario puede modificar
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ObjetoDto datos)
        {
            var objeto = await _context.Objetos.FindAsync(id);

            if (objeto == null)
                return NotFound();

            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            if (objeto.UsuarioId != usuarioId)
                return Forbid();

            if (datos.Tipo != "Perdido" &&
                datos.Tipo != "Encontrado")
                return BadRequest("Tipo no válido");

            if (datos.Estado != "Disponible" &&
                datos.Estado != "Recuperado")
                return BadRequest("Estado no válido");

            objeto.Titulo = datos.Titulo;
            objeto.Descripcion = datos.Descripcion;
            objeto.Tipo = datos.Tipo;
            objeto.Ubicacion = datos.Ubicacion;
            objeto.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: Solo el propietario puede eliminar
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var objeto = await _context.Objetos.FindAsync(id);

            if (objeto == null)
                return NotFound();

            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            if (objeto.UsuarioId != usuarioId)
                return Forbid();

            _context.Objetos.Remove(objeto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
