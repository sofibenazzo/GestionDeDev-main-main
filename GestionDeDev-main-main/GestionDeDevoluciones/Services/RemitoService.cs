
using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeDevoluciones.Services
{
    public class RemitoService : IRemitoService
    {
        private readonly AppDbContext _context;

        public RemitoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Remito> ObtenerTodos()
        {
            return _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.TipoEstado)
                .Include(r => r.Productos)
                .Include(r => r.Decision)
                .Include(r => r.Observacion)
                .ToList();
        }

        public Remito? ObtenerPorId(int id)
        {
            return _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.TipoEstado)
                .Include(r => r.Productos)
                .Include(r => r.Decision)
                .Include(r => r.Observacion)
                .FirstOrDefault(r => r.RemitoId == id);
        }

        public void Crear(Remito remito)
        {
            _context.Remitos.Add(remito);
            _context.SaveChanges();
        }

        public void Actualizar(Remito remito)
        {
            var existing = _context.Remitos.Find(remito.RemitoId);
            if (existing != null)
            {
                existing.ClienteId = remito.ClienteId;
                existing.TipoEstadoId = remito.TipoEstadoId;
                existing.Motivo = remito.Motivo;
                existing.ObservacionTexto = remito.ObservacionTexto;
                existing.ArchivoRemito = remito.ArchivoRemito;
                existing.ObservacionId = remito.ObservacionId;
                existing.DecisionAdoptadaId = remito.DecisionAdoptadaId;
                _context.SaveChanges();
            }
        }

        public void ActualizarEstado(int remitoId, int nuevoEstadoId)
        {
            var remito = _context.Remitos.Find(remitoId);
            if (remito != null)
            {
                remito.TipoEstadoId = nuevoEstadoId;
                _context.SaveChanges();
            }
        }
    }
}

