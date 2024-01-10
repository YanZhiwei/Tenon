using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Tenon.Helper.Internal
{
    public sealed class CompressHelper
    {
        public static byte[] Compress(byte[] data)
        {
            Checker.Begin().NotNull(data, nameof(data));
            using (var ms = new MemoryStream())
            {
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                var buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }


        /// <summary>
        ///     对byte 数组解压
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            Checker.Begin().NotNull(data, nameof(data));
            using (var tmpMs = new MemoryStream())
            {
                using (var ms = new MemoryStream(data))
                {
                    var zip = new GZipStream(ms, CompressionMode.Decompress, true);
                    zip.CopyTo(tmpMs);
                    zip.Close();
                }

                return tmpMs.ToArray();
            }
        }

        public static string Compress(string value)
        {
            Checker.Begin().NotNullOrEmpty(value, nameof(value));
            var bytes = Encoding.UTF8.GetBytes(value);
            bytes = Compress(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string Decompress(string value)
        {
            Checker.Begin().NotNullOrEmpty(value, nameof(value));
            var bytes = Convert.FromBase64String(value);
            bytes = Decompress(bytes);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}