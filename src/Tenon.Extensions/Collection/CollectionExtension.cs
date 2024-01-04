using System.Collections.Generic;
using System.Linq;

namespace Tenon.Extensions.Collection
{
    public static class CollectionExtension
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }
}