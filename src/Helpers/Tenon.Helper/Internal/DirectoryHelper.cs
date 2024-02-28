using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tenon.Helper.Internal
{
    public sealed class DirectoryHelper
    {
        public static IEnumerable<string> GetFiles(string folder, bool includedChildFolder = true, params string[] exts)
        {
            Checker.Begin().NotNullOrEmpty(folder, nameof(folder)).CheckDirectoryExist(folder)
                .NotNull(exts, nameof(exts));
            return Directory
                .EnumerateFiles(folder, "*.*",
                    includedChildFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s => exts.Contains(Path.GetExtension(s), StringComparer.OrdinalIgnoreCase));
        }
    }
}