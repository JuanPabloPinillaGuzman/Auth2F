namespace Application.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Enable2FA { get; set; }
    }
}
