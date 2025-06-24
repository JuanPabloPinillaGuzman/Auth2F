using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web; // Para HttpUtility.UrlEncode

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiAuth2FDBContext _context;
        // Temporal tokens para demo (en prod usar JWT o cache distribuido)
        private static ConcurrentDictionary<string, string> _tempTokens = new();

        public AuthService(ApiAuth2FDBContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return new LoginResponse { Requires2FA = false, Message = "Usuario o contraseña inválidos" };

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new LoginResponse { Requires2FA = false, Message = "Usuario o contraseña inválidos" };

            if (user.Is2FAEnabled && !string.IsNullOrEmpty(user.TwoFactorSecret))
            {
                // Generar token temporal
                var tempToken = Guid.NewGuid().ToString();
                _tempTokens[tempToken] = user.Email;
                return new LoginResponse { Requires2FA = true, Message = "Ingrese el código 2FA", TempToken = tempToken };
            }

            // Autenticación exitosa sin 2FA
            return new LoginResponse { Requires2FA = false, Message = "Login exitoso", TempToken = GenerateAuthToken(user) };
        }

        public async Task<TwoFactorResponse> Verify2FAAsync(TwoFactorRequest request)
        {
            if (!_tempTokens.TryGetValue(request.TempToken, out var email) || email != request.Email)
                return new TwoFactorResponse { Success = false, Message = "Token temporal inválido" };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
                return new TwoFactorResponse { Success = false, Message = "Usuario inválido" };

            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));
            bool valid = totp.VerifyTotp(request.Code, out long timeStepMatched, new VerificationWindow(2, 2));
            if (!valid)
                return new TwoFactorResponse { Success = false, Message = "Código 2FA inválido" };

            // Eliminar token temporal usado
            _tempTokens.TryRemove(request.TempToken, out _);
            return new TwoFactorResponse { Success = true, Message = "Autenticación exitosa", AuthToken = GenerateAuthToken(user) };
        }

        // Registro de usuario y activación de 2FA
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return new RegisterResponse { Message = "El email ya está registrado." };

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            string? secret = null;
            string? qrUrl = null;
            bool enable2FA = request.Enable2FA;
            if (enable2FA)
            {
                secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
                string issuer = "Auth2F";
                string label = HttpUtility.UrlEncode(request.Email);
                string encodedIssuer = HttpUtility.UrlEncode(issuer);
                qrUrl = $"otpauth://totp/{encodedIssuer}:{label}?secret={secret}&issuer={encodedIssuer}";
            }
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Is2FAEnabled = enable2FA,
                TwoFactorSecret = secret
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new RegisterResponse
            {
                Message = enable2FA ? "Usuario registrado. Escanee el QR en Google Authenticator." : "Usuario registrado.",
                TwoFactorSecret = secret,
                QrCodeUrl = qrUrl
            };
        }

        // Para demo: generar un token simple (en prod usar JWT)
        private string GenerateAuthToken(User user)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Email}:{DateTime.UtcNow.Ticks}"));
        }
    }
}
