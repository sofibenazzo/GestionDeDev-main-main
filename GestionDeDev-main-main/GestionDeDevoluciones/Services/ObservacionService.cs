using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class ObservacionService : IObservacionService
    {
        private readonly AppDbContext _context;

        public ObservacionService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Observaciones> ObtenerTodos()
        {
            return _context.Observaciones.ToList();
        }

        public void Crear(Observaciones observacion)
        {
            _context.Observaciones.Add(observacion);
            _context.SaveChanges();
        }
    }
}
