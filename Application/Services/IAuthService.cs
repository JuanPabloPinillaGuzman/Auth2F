using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<TwoFactorResponse> Verify2FAAsync(TwoFactorRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
