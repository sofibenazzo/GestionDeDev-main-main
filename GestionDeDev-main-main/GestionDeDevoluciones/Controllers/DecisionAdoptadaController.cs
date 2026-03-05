using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionAdoptadaController : ControllerBase
    {
        private readonly IDecisionAdoptadaService _service;

        public DecisionAdoptadaController(IDecisionAdoptadaService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Post([FromBody] DecisionAdoptada decision)
        {
            _service.Crear(decision);
            return Ok(decision);
        }
    }
}
