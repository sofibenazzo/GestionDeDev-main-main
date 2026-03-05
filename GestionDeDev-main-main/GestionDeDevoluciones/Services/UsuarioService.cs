using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            return _context.Usuarios.ToList();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _context.Usuarios.Find(id);
        }

        public void Crear(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Actualizar(int id, Usuario usuario)
        {
            var existing = _context.Usuarios.Find(id);
            if (existing != null)
            {
                existing.Nombre = usuario.Nombre;
                existing.Email = usuario.Email;
                existing.RolId = usuario.RolId;
                if (!string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    existing.PasswordHash = usuario.PasswordHash;
                }
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
        }
    }
}
