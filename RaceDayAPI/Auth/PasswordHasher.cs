namespace RaceDayAPI.Auth
{
    public class PasswordHasher
    {
        public static string Hash(string plainPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainPassword);

        public static bool Verify(string plainPassword, string hash) =>
            BCrypt.Net.BCrypt.Verify(plainPassword, hash);
    }
}
