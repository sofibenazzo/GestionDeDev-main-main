using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductoController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var p = _service.ObtenerPorId(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Producto producto)
        {
            _service.Crear(producto);
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Producto producto)
        {
            if (id != producto.ProductoId) return BadRequest();
            _service.Actualizar(producto);
            return Ok(producto);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Eliminar(id);
            return Ok();
        }
    }
}
