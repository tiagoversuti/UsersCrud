namespace Users.Business
{
    public class Hash
    {
        private const int HASH_WORK_FACTOR = 12;

        public static bool Verify(string value, string salt)
        {
            return BCrypt.Net.BCrypt.Verify(value, salt);
        }

        public static string Generate(string value)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt(HASH_WORK_FACTOR);
            return BCrypt.Net.BCrypt.HashPassword(value, salt);
        }
    }
}
