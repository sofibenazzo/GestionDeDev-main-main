using GestionDeDevoluciones.Models;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_clienteService.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var cliente = _clienteService.ObtenerPorId(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Cliente cliente)
        {
            _clienteService.Crear(cliente);
            return Ok(cliente);
        }
    }
}
