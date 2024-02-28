using System.IO;

namespace Tenon.Helper.Internal
{

    public sealed class FileHelper
    {
        public string RemoveFileNameInvalidChars(string filename)
        {
            return string.IsNullOrWhiteSpace(filename) ? null : string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public string ReplaceFileNameInvalidChars(string filename, string separator = "_")
        {
            return string.IsNullOrWhiteSpace(filename)
                ? null
                : string.Join(separator, filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}