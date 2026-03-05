
using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class RolService : IRolService
    {
        private readonly AppDbContext _context;

        public RolService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Rol> ObtenerTodos()
        {
            return _context.Roles.ToList();
        }

        public void Crear(Rol rol)
        {
            _context.Roles.Add(rol);
            _context.SaveChanges();
        }
    }
}
