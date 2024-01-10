using System;
using System.Linq;

namespace Tenon.Helper.Internal
{
    public static class AttributeHelper
    {
        public static A Get<T, A>()
            where T : class
            where A : Attribute
        {
            var modelType = typeof(T);

            var modelAttrs = modelType.GetCustomAttributes(typeof(A), true);

            var any = modelAttrs?.Any() ?? false;

            return any ? modelAttrs.FirstOrDefault() as A : null;
        }
    }
}