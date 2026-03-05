
using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class TipoEstadoService : ITipoEstadoService
    {
        private readonly AppDbContext _context;

        public TipoEstadoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoEstado> ObtenerTodos()
        {
            return _context.TiposEstado.ToList();
        }

        public void Crear(TipoEstado tipoEstado)
        {
            _context.TiposEstado.Add(tipoEstado);
            _context.SaveChanges();
        }
    }
}