using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObservacionController : ControllerBase
    {
        private readonly IObservacionService _service;

        public ObservacionController(IObservacionService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Observaciones observacion)
        {
            _service.Crear(observacion);
            return Ok();
        }
    }
}
