using System.ComponentModel.DataAnnotations;

namespace GestionDeDevoluciones.Models
{
    public class Direccion
    {
        [Key]
        public int DireccionId { get; set; }

        [Required, StringLength(100)]
        public string Calle { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string Numero { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string Ciudad { get; set; } = string.Empty;
    }
}
