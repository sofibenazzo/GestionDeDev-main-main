
using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class DecisionAdoptadaService : IDecisionAdoptadaService
    {
        private readonly AppDbContext _context;

        public DecisionAdoptadaService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<DecisionAdoptada> ObtenerTodos()
        {
            return _context.DecisionesAdoptadas.ToList();
        }

        public void Crear(DecisionAdoptada decision)
        {
            _context.DecisionesAdoptadas.Add(decision);
            _context.SaveChanges();
        }
    }
}
