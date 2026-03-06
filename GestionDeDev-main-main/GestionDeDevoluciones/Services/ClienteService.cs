using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            // ✅ CORREGIDO: Include para traer la Dirección junto al Cliente
            return _context.Clientes
                .Include(c => c.Direccion)
                .ToList();
        }

        public Cliente? ObtenerPorId(int id)
        {
            // ✅ CORREGIDO: Include para traer la Dirección junto al Cliente
            return _context.Clientes
                .Include(c => c.Direccion)
                .FirstOrDefault(c => c.ClienteId == id);
        }

        public void Crear(Cliente cliente)
        {
            // ✅ CORREGIDO: si la dirección viene pero todos sus campos están vacíos,
            // no la guardamos (evita registros inútiles en la tabla Direcciones)
            if (cliente.Direccion != null)
            {
                bool direccionVacia =
                    string.IsNullOrWhiteSpace(cliente.Direccion.Calle) &&
                    string.IsNullOrWhiteSpace(cliente.Direccion.Numero) &&
                    string.IsNullOrWhiteSpace(cliente.Direccion.Ciudad);

                if (direccionVacia)
                {
                    cliente.Direccion = null;
                    cliente.DireccionId = null;
                }
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }
    }
}