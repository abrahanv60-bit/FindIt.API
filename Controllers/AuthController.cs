
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FindIt.API.Data;
using FindIt.API.Models;
using FindIt.API.DTOs;

namespace FindIt.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Usuario> _hasher;
        private readonly IConfiguration _config;

        public AuthController(
            AppDbContext context,
            IPasswordHasher<Usuario> hasher,
            IConfiguration config)
        {
            _context = context;
            _hasher = hasher;
            _config = config;
        }

        // POST: Registrar usuario
        [HttpPost("registro")]
        public async Task<IActionResult> Registro(RegistroDto datos)
        {
            string email = datos.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(datos.Nombre))
                return BadRequest("El nombre es obligatorio");

            bool existe = await _context.Usuarios
                .AnyAsync(u => u.Email == email);

            if (existe)
                return Conflict("El correo ya está registrado");

            var usuario = new Usuario
            {
                Nombre = datos.Nombre.Trim(),
                Email = email,
                Rol = "Usuario"
            };

            usuario.PasswordHash =
                _hasher.HashPassword(usuario, datos.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol
            });
        }

        // POST: Iniciar sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto datos)
        {
            string email = datos.Email.Trim().ToLowerInvariant();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || usuario.PasswordHash == null)
                return Unauthorized("Credenciales incorrectas");

            var resultado = _hasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                datos.Password
            );

            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized("Credenciales incorrectas");

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.PasswordHash =
                    _hasher.HashPassword(usuario, datos.Password);
                await _context.SaveChangesAsync();
            }

            // Crear token JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var credenciales = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Rol
                }
            });
        }

        // GET: Obtener perfil del usuario autenticado
        [Authorize]
        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            return Ok(new
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Rol = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
