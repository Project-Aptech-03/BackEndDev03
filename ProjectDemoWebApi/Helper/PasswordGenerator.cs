namespace ProjectDemoWebApi.Helper
{
    public static class PasswordGenerator
    {
        private static readonly Random _random = new();
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Specials = "!@#$%^&*()_+?~";

        public static string GeneratePassword(int length = 10)
        {
            string allChars = Upper + Lower + Digits + Specials;

            var passwordChars = new List<char>
        {
            Upper[_random.Next(Upper.Length)],
            Lower[_random.Next(Lower.Length)],
            Digits[_random.Next(Digits.Length)],
            Specials[_random.Next(Specials.Length)]
        };

            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(allChars[_random.Next(allChars.Length)]);
            }

            return new string(passwordChars.OrderBy(x => _random.Next()).ToArray());
        }
    }

}
