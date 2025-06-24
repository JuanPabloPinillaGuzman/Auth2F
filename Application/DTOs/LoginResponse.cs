namespace Application.DTOs
{
    public class LoginResponse
    {
        public bool Requires2FA { get; set; }
        public string Message { get; set; }
        public string? TempToken { get; set; }
    }
}
