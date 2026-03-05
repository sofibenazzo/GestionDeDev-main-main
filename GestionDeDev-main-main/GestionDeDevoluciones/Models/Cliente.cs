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

        [Required, StringLength(50)]
        public string CodigoCliente { get; set; } = string.Empty;

        // FK explícita
        public int DireccionId { get; set; }

        // Navegación
        public Direccion? Direccion { get; set; }
    }
}
