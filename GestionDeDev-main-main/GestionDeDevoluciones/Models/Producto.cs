using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeDevoluciones.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        public int RemitoId { get; set; }

        [Required, StringLength(150)]
        public string Item { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; }

        [StringLength(80)]
        public string? CV { get; set; }

        [StringLength(80)]
        public string? RPM { get; set; }

        [StringLength(50)]
        public string? Voltaje { get; set; }

        [StringLength(120)]
        public string? Protec { get; set; }

        [StringLength(120)]
        public string? Const { get; set; }

        [StringLength(120)]
        public string? Modelo { get; set; }

        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        [ForeignKey(nameof(RemitoId))]
        public Remito? Remito { get; set; }
    }
}
