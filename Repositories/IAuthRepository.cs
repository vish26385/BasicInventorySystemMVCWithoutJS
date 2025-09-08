using ALLINONEPROJECTWITHOUTJS.DTOs;
using ALLINONEPROJECTWITHOUTJS.Models;

namespace ALLINONEPROJECTWITHOUTJS.Repositories
{
    public interface IAuthRepository
    {
        Task<int> RegisterAsync(RegisterRequest request);
        Task<User?> ForgotPasswordAsync(string email);
        Task<int> LoginAsync(LoginRequest request);
    }
}
