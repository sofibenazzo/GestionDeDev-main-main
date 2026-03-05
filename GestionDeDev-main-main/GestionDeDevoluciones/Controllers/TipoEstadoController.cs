using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoEstadoController : ControllerBase
    {
        private readonly ITipoEstadoService _service;

        public TipoEstadoController(ITipoEstadoService service)
        {
            _service = service;
        }

        // GET: api/tipoestado
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.ObtenerTodos());
        }

        // POST: api/tipoestado
        [HttpPost]
        public IActionResult Post([FromBody] TipoEstado tipoEstado)
        {
            _service.Crear(tipoEstado);
            return Ok();
        }
    }
}
