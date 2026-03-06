using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeDevoluciones.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required, StringLength(150)]
        public string RazonSocial { get; set; } = string.Empty;

        // ✅ CORREGIDO: opcional, sin [Required]
        [StringLength(50)]
        public string? CodigoCliente { get; set; }

        // ✅ CORREGIDO: FK nullable, la dirección es opcional
        public int? DireccionId { get; set; }

        public Direccion? Direccion { get; set; }
    }
}