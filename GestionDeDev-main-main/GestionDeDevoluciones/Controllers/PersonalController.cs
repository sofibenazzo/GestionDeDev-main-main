using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalController : ControllerBase
    {
        private readonly IPersonalService _personalService;

        public PersonalController(IPersonalService personalService)
        {
            _personalService = personalService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_personalService.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Personal personal)
        {
            _personalService.Crear(personal);
            return Ok();
        }
    }
}
