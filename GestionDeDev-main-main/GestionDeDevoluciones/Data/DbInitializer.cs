using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GestionDeDevoluciones.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("➡️ DbInitializer iniciado.");
            try 
            {
                // context.Database.EnsureDeleted(); // COMENTADO para evitar pérdida de datos
                context.Database.EnsureCreated();
                
                try { context.Database.ExecuteSqlRaw("ALTER TABLE Observaciones ADD Descripcion nvarchar(500) DEFAULT '' NOT NULL"); } catch {}
                try { context.Database.ExecuteSqlRaw("ALTER TABLE DecisionesAdoptadas ADD Descripcion nvarchar(max) DEFAULT '' NOT NULL"); } catch {}

                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<Rol> { new Rol { Nombre = "Administrador" }, new Rol { Nombre = "Usuario" } });
                    context.SaveChanges();
                }

                if (!context.TiposEstado.Any())
                {
                    context.TiposEstado.AddRange(new List<TipoEstado> { 
                        new TipoEstado { Estado = "Pendiente" }, 
                        new TipoEstado { Estado = "En Revisión" }, 
                        new TipoEstado { Estado = "Aceptada" }, 
                        new TipoEstado { Estado = "Rechazada" } 
                    });
                    context.SaveChanges();
                }

                if (!context.Personal.Any())
                {
                    context.Personal.Add(new Personal { Nombre = "Técnico WEG", Legajo = "T-001" });
                    context.Personal.Add(new Personal { Nombre = "Admin Facturación", Legajo = "F-001" });
                    context.SaveChanges();
                }

                if (!context.Clientes.Any())
                {
                    var dir = new Direccion { Calle = "Bv. 25 de Mayo", Numero = "1234", Ciudad = "San Francisco" };
                    context.Direcciones.Add(dir);
                    context.SaveChanges();

                    context.Clientes.Add(new Cliente { RazonSocial = "WEG Argentina S.A.", CodigoCliente = "CLI-WEG-01", DireccionId = dir.DireccionId });
                    context.SaveChanges();
                }

                if (!context.Usuarios.Any())
                {
                    var adminRole = context.Roles.FirstOrDefault(r => r.Nombre == "Administrador");
                    if (adminRole != null) context.Usuarios.Add(new Usuario { Nombre = "Administrador", Email = "admin@weg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), RolId = adminRole.RolId });
                    context.SaveChanges();
                }
                Console.WriteLine("✅ Seeding finalizado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR CRÍTICO EN INITIALIZE: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }
    }
}
