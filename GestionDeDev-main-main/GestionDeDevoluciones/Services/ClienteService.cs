using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using GestionDeDevoluciones.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GestionDeDevoluciones.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _context.Clientes.ToList();
        }

        public Cliente? ObtenerPorId(int id)
        {
            return _context.Clientes.FirstOrDefault(c => c.ClienteId == id);
        }

        public void Crear(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }
    }
}
