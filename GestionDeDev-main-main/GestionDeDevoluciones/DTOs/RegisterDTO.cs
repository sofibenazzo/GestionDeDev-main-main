using System.ComponentModel.DataAnnotations;

namespace GestionDeDevoluciones.Backend.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }
    }
}
