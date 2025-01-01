namespace MaReSy2.Services
{
    public class TokenManager
    {
            // Speichere das Token in einer statischen Variable
            private static string _token;

            // Methode zum Abrufen des Tokens
            public static string GetToken()
            {
                return _token;
            }

            // Methode zum Setzen des Tokens
            public static void SetToken(string token)
            {
                _token = token;
            }
    }
}
