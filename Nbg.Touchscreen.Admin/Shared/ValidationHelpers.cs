using System.Text.RegularExpressions;

namespace Nbg.Touchscreen.Admin.Shared
{
    public static class ValidationHelpers
    {
        private static readonly Regex _emailRx = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string? email)
            => !string.IsNullOrWhiteSpace(email) && _emailRx.IsMatch(email!);

        // password policy: min 10, at least 1 uppercase, at least 1 digit
        private static readonly Regex _upperRx = new Regex("[A-Z]", RegexOptions.Compiled);
        private static readonly Regex _digitRx = new Regex(@"\d", RegexOptions.Compiled);

        public static bool IsValidPassword(string? password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password!.Length < 10) return false;
            if (!_upperRx.IsMatch(password)) return false;
            if (!_digitRx.IsMatch(password)) return false;
            return true;
        }
    }
}
