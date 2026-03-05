using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeDevoluciones.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required, StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol? Rol { get; set; }
    }
}
