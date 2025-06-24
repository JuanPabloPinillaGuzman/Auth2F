using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Services
{
    // La implementación real irá en Infrastructure
    public class AuthService : IAuthService
    {
        public Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<TwoFactorResponse> Verify2FAAsync(TwoFactorRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
