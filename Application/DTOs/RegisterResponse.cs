namespace Application.DTOs
{
    public class RegisterResponse
    {
        public string Message { get; set; }
        public string? TwoFactorSecret { get; set; }
        public string? QrCodeUrl { get; set; }
    }
}
