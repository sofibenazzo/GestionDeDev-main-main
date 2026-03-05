using GestionDeDevoluciones.Backend.DTOs;

namespace GestionDeDevoluciones.Backend.Auth
{
    public interface IAuthService
    {
        string Register(RegisterDTO dto);
        string Login(LoginDTO dto);
    }
}
