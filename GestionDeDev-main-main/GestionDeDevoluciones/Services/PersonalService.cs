
using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly AppDbContext _context;

        public PersonalService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Personal> ObtenerTodos()
        {
            return _context.Personal.ToList();
        }

        public Personal? ObtenerPorId(int id)
        {
            return _context.Personal.Find(id);
        }

        public void Crear(Personal personal)
        {
            _context.Personal.Add(personal);
            _context.SaveChanges();
        }
    }
}
