
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
    [Authorize]
    [Route("api/solicitudes")]
    public class SolicitudesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SolicitudesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ver solicitudes relacionadas con mi usuario
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var solicitudes = await _context.Solicitudes
                .Where(s => s.UsuarioId == usuarioId ||
                    _context.Objetos.Any(o =>
                        o.Id == s.ObjetoId &&
                        o.UsuarioId == usuarioId))
                .ToListAsync();

            return Ok(solicitudes);
        }

        // GET: Buscar solicitud por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var solicitud = await _context.Solicitudes.FindAsync(id);

            if (solicitud == null)
                return NotFound();

            var objeto = await _context.Objetos
                .FindAsync(solicitud.ObjetoId);

            if (solicitud.UsuarioId != usuarioId &&
                objeto?.UsuarioId != usuarioId)
                return NotFound();

            return Ok(solicitud);
        }

        // POST: Crear solicitud
        [HttpPost]
        public async Task<IActionResult> Post(CrearSolicitudDto datos)
        {
            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var objeto = await _context.Objetos
                .FindAsync(datos.ObjetoId);

            if (objeto == null)
                return NotFound("El objeto no existe");

            if (objeto.UsuarioId == usuarioId)
                return BadRequest(
                    "No puedes solicitar tu propio objeto"
                );

            if (objeto.Estado != "Disponible")
                return BadRequest(
                    "El objeto ya no está disponible"
                );

            if (string.IsNullOrWhiteSpace(datos.Mensaje))
                return BadRequest(
                    "El mensaje es obligatorio"
                );

            var solicitud = new Solicitud
            {
                ObjetoId = datos.ObjetoId,
                UsuarioId = usuarioId,
                Mensaje = datos.Mensaje.Trim(),
                Estado = "Pendiente",
                FechaSolicitud = DateTime.UtcNow
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPorId),
                new { id = solicitud.Id },
                solicitud
            );
        }

        // PATCH: Aprobar o rechazar solicitud
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(
            int id, CambiarEstadoDto datos)
        {
            int usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var solicitud = await _context.Solicitudes
                .FindAsync(id);

            if (solicitud == null)
                return NotFound("Solicitud no encontrada");

            var objeto = await _context.Objetos
                .FindAsync(solicitud.ObjetoId);

            if (objeto == null)
                return NotFound("Objeto no encontrado");

            // Solo el propietario puede decidir
            if (objeto.UsuarioId != usuarioId)
                return Forbid();

            if (solicitud.Estado != "Pendiente")
                return BadRequest(
                    "La solicitud ya fue procesada"
                );

            if (datos.Estado != "Aprobada" &&
                datos.Estado != "Rechazada")
                return BadRequest("Estado no válido");

            if (objeto.Estado != "Disponible")
                return BadRequest(
                    "El objeto ya no está disponible"
                );

            solicitud.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return Ok(solicitud);
        }
    }
}
