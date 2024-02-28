using System.Linq;
using Tenon.Extensions.System;

namespace Tenon.Extensions.Collection;

public static class ArrayExtension
{
    public static bool ContainIgnoreCase(this string[] sourceArray, string compareStringItem)
    {
        return sourceArray.Any(item => item.CompareIgnoreCase(compareStringItem));
    }
}