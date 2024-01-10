using System.Text.RegularExpressions;

namespace Tenon.Helper.Internal
{
    public sealed class ExcelHelper
    {
        public static string RemoveSheetNameInvalidChars(string sheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetName)) return null;
            const string invalidCharsRegex = @"[/\\*'?[\]:]+";
            const int maxLength = 31;

            var safeName = Regex.Replace(sheetName, invalidCharsRegex, " ")
                .Replace("  ", " ")
                .Trim();

            if (safeName.Length > maxLength) safeName = safeName.Substring(0, maxLength);

            return safeName;
        }
    }
}