using System.Text.RegularExpressions;

namespace Tenon.Helper.Internal
{
    public static class RegexHelper
    {
        public static bool IsMatch(string checkString, string regexString)
        {
            return IsMatch(checkString, regexString, RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(string checkString, string regexString, RegexOptions options)
        {
            return Regex.IsMatch(checkString, regexString, options);
        }


        public static bool IsMatch(string checkString, string regexString, out Match result)
        {
            result = null;
            var regex = new Regex(regexString);
            result = regex.Match(checkString);
            return result.Success;
        }
    }
}