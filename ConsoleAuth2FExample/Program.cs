using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

static async Task Main(string[] args)
{
    var baseUrl = "https://localhost:5001"; // Cambia el puerto si tu API usa otro
    var client = new HttpClient();

    Console.WriteLine("=== Registro de usuario y activación de 2FA ===");
    Console.Write("Email: ");
    var email = Console.ReadLine();
    Console.Write("Contraseña: ");
    var password = ReadPassword();
    Console.Write("¿Activar 2FA? (s/n): ");
    var enable2FA = Console.ReadLine()?.Trim().ToLower() == "s";

    var registerRequest = new RegisterRequest
    {
        Email = email,
        Password = password,
        Enable2FA = enable2FA
    };
    try
    {
        var regResp = await client.PostAsJsonAsync($"{baseUrl}/api/auth/register", registerRequest);
        if (!regResp.IsSuccessStatusCode)
        {
            var error = await regResp.Content.ReadAsStringAsync();
            Console.WriteLine($"Error en registro: {regResp.StatusCode} - {error}");
            return;
        }
        var registerResult = await regResp.Content.ReadFromJsonAsync<RegisterResponse>();
        Console.WriteLine($"Mensaje: {registerResult.Message}");
        if (enable2FA)
        {
            Console.WriteLine($"Secreto 2FA: {registerResult.TwoFactorSecret}");
            Console.WriteLine($"URL QR: {registerResult.QrCodeUrl}");
            Console.WriteLine("Escanea el QR con Google Authenticator antes de continuar...");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Excepción en registro: {ex.Message}");
        return;
    }
    Console.WriteLine("Presiona Enter para hacer login...");
    Console.ReadLine();

    Console.WriteLine("=== Login ===");
    Console.Write("Email: ");
    email = Console.ReadLine();
    Console.Write("Contraseña: ");
    password = ReadPassword();
    var loginRequest = new LoginRequest { Email = email, Password = password };
    LoginResponse loginResult = null;
    try
    {
        var loginResp = await client.PostAsJsonAsync($"{baseUrl}/api/auth/login", loginRequest);
        if (!loginResp.IsSuccessStatusCode)
        {
            var error = await loginResp.Content.ReadAsStringAsync();
            Console.WriteLine($"Error en login: {loginResp.StatusCode} - {error}");
            return;
        }
        loginResult = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();
        Console.WriteLine($"Mensaje: {loginResult.Message}");
        if (!loginResult.Requires2FA)
        {
            Console.WriteLine($"Token de autenticación: {loginResult.TempToken}");
            return;
        }
        Console.WriteLine($"TempToken: {loginResult.TempToken}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Excepción en login: {ex.Message}");
        return;
    }

    Console.WriteLine("Ingresa el código de 6 dígitos generado por Google Authenticator:");
    var code = Console.ReadLine();

    Console.WriteLine("=== Verificar 2FA ===");
    var twoFaRequest = new TwoFactorRequest
    {
        Email = email,
        Code = code,
        TempToken = loginResult.TempToken
    };
    try
    {
        var twoFaResp = await client.PostAsJsonAsync($"{baseUrl}/api/auth/verify-2fa", twoFaRequest);
        if (!twoFaResp.IsSuccessStatusCode)
        {
            var error = await twoFaResp.Content.ReadAsStringAsync();
            Console.WriteLine($"Error en 2FA: {twoFaResp.StatusCode} - {error}");
            return;
        }
        var twoFaResult = await twoFaResp.Content.ReadFromJsonAsync<TwoFactorResponse>();
        Console.WriteLine($"Mensaje: {twoFaResult.Message}");
        Console.WriteLine($"¿Acceso?: {twoFaResult.Success}");
        Console.WriteLine($"AuthToken: {twoFaResult.AuthToken}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Excepción en 2FA: {ex.Message}");
    }
}

// Método para leer contraseña oculta
private static string ReadPassword()
{
    string pwd = "";
    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;
        if (key == ConsoleKey.Backspace && pwd.Length > 0)
        {
            pwd = pwd[..^1];
            Console.Write("\b \b");
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            pwd += keyInfo.KeyChar;
            Console.Write("*");
        }
    } while (key != ConsoleKey.Enter);
    Console.WriteLine();
    return pwd;
}
