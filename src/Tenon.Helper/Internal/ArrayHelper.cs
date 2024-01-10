using System;

namespace Tenon.Helper.Internal
{
    public sealed class ArrayHelper
    {
        public static T[] Copy<T>(T[] sourceArray, int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            var destination = new T[len];
            Array.Copy(sourceArray, startIndex, destination, 0, len);
            return destination;
        }
    }
}