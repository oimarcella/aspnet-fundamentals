namespace Blog;
public static class Configuration
{
    public static string JwtKey = "RozOU+mjqkaZAvilStv+Uw==RozOU+mjqkaZAvilStv+Uw==";
    public static string ApiKeyName = "api_key";
    public static string ApiKeyValue = "curso_api_a3343faf-acc4-4e2e-9dd1-e2f0e972ef3e";
    public static int AppPort = 0;
    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
