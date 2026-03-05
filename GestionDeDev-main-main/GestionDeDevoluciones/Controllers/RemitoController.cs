using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemitoController : ControllerBase
    {
        private readonly IRemitoService _remitoService;
        private readonly IWebHostEnvironment _env;

        public RemitoController(IRemitoService remitoService, IWebHostEnvironment env)
        {
            _remitoService = remitoService;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try 
            {
                var remitos = _remitoService.ObtenerTodos();
                return Ok(remitos);
            }
            catch (System.Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"ERROR AL OBTENER REMITOS: {errorMsg}");
                return StatusCode(500, new { error = errorMsg });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var remito = _remitoService.ObtenerPorId(id);
            if (remito == null) return NotFound();
            return Ok(remito);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Remito remito)
        {
            try 
            {
                if (remito == null) return BadRequest("El remito no puede ser nulo.");
                if (remito.Fecha == default) remito.Fecha = System.DateTime.Now;

                _remitoService.Crear(remito);
                return Ok(new { remitoId = remito.RemitoId });
            }
            catch (System.Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"ERROR AL GUARDAR REMITO: {errorMsg}");
                return StatusCode(500, new { error = errorMsg });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Remito remito)
        {
            try
            {
                remito.RemitoId = id;
                _remitoService.Actualizar(remito);
                return Ok();
            }
            catch (System.Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = errorMsg });
            }
        }

        [HttpPut("{id}/estado")]
        public IActionResult UpdateStatus(int id, [FromBody] int nuevoEstadoId)
        {
            _remitoService.ActualizarEstado(id, nuevoEstadoId);
            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se recibió ningún archivo.");

                var ext = Path.GetExtension(archivo.FileName).ToLower();
                if (ext != ".pdf" && ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                    return BadRequest("Solo se permiten archivos PDF, PNG o JPG.");

                var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                return Ok(new { fileName = uniqueName, filePath = $"/uploads/{uniqueName}" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
