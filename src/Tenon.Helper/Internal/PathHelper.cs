using System;
using System.IO;

namespace Tenon.Helper.Internal
{
    public sealed class PathHelper
    {
        public static bool IsSame(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(path2)) return false;

            try
            {
                var path3 = Path.GetFullPath(path1);
                var path4 = Path.GetFullPath(path2);
                return string.Equals(path3, path4, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ReplaceInvalidChars(string path, string separator = "_")
        {
            return string.IsNullOrWhiteSpace(path) ? null : string.Join(separator, path.Split(Path.GetInvalidPathChars()));
        }

        public string RemoveInvalidChars(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? null : string.Concat(path.Split(Path.GetInvalidPathChars()));
        }
    }
}