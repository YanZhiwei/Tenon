using System;
using System.Collections.Generic;
using System.Linq;

namespace Tenon.Helper.Internal
{
    public static class EnumHelper
    {
        #region Methods

        public static bool ContainEnumName<T>(string enumName)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return false;
            var array = Enum.GetNames(typeof(T));

            bool? any = array.Any();

            if (!(bool)any) return false;

            return array.Any(item => string.Compare(item, enumName, StringComparison.OrdinalIgnoreCase) == 0);
        }


        public static string GetName<T>(int enumNumber)
            where T : struct, IConvertible
        {
            return Enum.GetName(typeof(T), enumNumber);
        }


        public static T[] GetValues<T>(Type enumType)
        {
            if (!enumType.IsEnum) return null;
            var array = Enum.GetValues(enumType);
            var count = array.Length;
            var values = new T[count];

            for (var i = 0; i < count; i++) values[i] = (T)array.GetValue(i);

            return values;
        }


        public static T ParseEnumName<T>(string enumName)
            where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), enumName, true);
        }


        public static List<KeyValuePair<int, string>> ToKeyValuePair<T>()
            where T : struct, IConvertible
        {
            var keyValues = new List<KeyValuePair<int, string>>();
            if (!typeof(T).IsEnum) return keyValues;
            keyValues.AddRange(Enum.GetNames(typeof(T))
                .Select(item => new KeyValuePair<int, string>((int)Enum.Parse(typeof(T), item), item)));
            return keyValues;
        }

        #endregion Methods
    }
}