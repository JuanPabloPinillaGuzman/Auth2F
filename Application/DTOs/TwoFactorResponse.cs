namespace Application.DTOs
{
    public class TwoFactorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? AuthToken { get; set; }
    }
}
