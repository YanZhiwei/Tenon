using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace Tenon.Helper.Internal
{
    public static class ConvertHelper
    {
        #region Methods

        public static bool ToBooleanOrDefault(this object data, bool defaultVal = false)
        {
            return data != null && bool.TryParse(data.ToString(), out var result) ? result : defaultVal;
        }


        public static byte ToByteOrDefault(this object data, byte defaultVal = 0x00)
        {
            return data != null && byte.TryParse(data.ToString(), out var result) ? result : defaultVal;
        }


        public static string ToChineseDate(this DateTime date)
        {
            var cnDate = new ChineseLunisolarCalendar();
            string[] months = { string.Empty, "正月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "冬月", "腊月" };
            string[] days =
            {
                string.Empty, "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十", "十一", "十二", "十三", "十四", "十五",
                "十六", "十七", "十八", "十九", "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
            };
            string[] years =
            {
                string.Empty, "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉", "甲戌", "乙亥", "丙子", "丁丑", "戊寅",
                "己卯", "庚辰", "辛己", "壬午", "癸未", "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳", "甲午", "乙未",
                "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑", "壬寅", "癸丑", "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子",
                "癸丑", "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥"
            };
            var year = cnDate.GetYear(date);
            var yearCn = years[cnDate.GetSexagenaryYear(date)];
            int month = cnDate.GetMonth(date),
                day = cnDate.GetDayOfMonth(date),
                leapMonth = cnDate.GetLeapMonth(year);
            var monthCn = months[month];

            if (leapMonth <= 0) return $"{yearCn}年{monthCn}{days[day]}";
            monthCn = month == leapMonth ? $"闰{months[month - 1]}" : monthCn;
            monthCn = month > leapMonth ? months[month - 1] : monthCn;

            return $"{yearCn}年{monthCn}{days[day]}";
        }


        public static string ToChineseDay(int data)
        {
            var result = string.Empty;

            if (data == 0 || data > 32) return result;
            string[] days =
            {
                "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四", "十五", "十六", "十七",
                "十八", "十九", "廿十", "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十", "三十一"
            };
            result = days[data];

            return result;
        }


        public static string ToChineseMonth(this int data)
        {
            var result = string.Empty;

            if (data == 0 || data > 12) return result;
            string[] months = { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
            result = months[data];

            return result;
        }


        public static DateTime ToDateOrDefault(this object data, DateTime defaultVal)
        {
            return DateTime.TryParse(data.ToString(), out var result) ? result : defaultVal;
        }


        public static DateTime ToDateOrDefault(this object data)
        {
            return ToDateOrDefault(data, DateTime.MinValue);
        }


        public static decimal ToDecimalOrDefault(this object data, decimal defaultVal = 0m)
        {
            if (data != null)
            {
                var result = decimal.TryParse(data.ToString(), out var parseDecimalValue);
                return result ? parseDecimalValue : defaultVal;
            }

            return defaultVal;
        }


        public static double ToDoubleOrDefault(this object data, double defaultVal = 0d)
        {
            if (data == null) return defaultVal;
            var result = double.TryParse(data.ToString(), out var parseIntValue);
            return result ? parseIntValue : defaultVal;
        }


        public static int ToInt32OrDefault(this object data, int defaultVal = 0)
        {
            if (data == null) return defaultVal;
            var result = int.TryParse(data.ToString(), out var parseIntValue);
            return result ? parseIntValue : defaultVal;
        }


        public static long ToInt64OrDefault(this object data, long defaultVal = 0)
        {
            if (data == null) return defaultVal;
            var result = long.TryParse(data.ToString(), out var parseIntValue);
            return result ? parseIntValue : defaultVal;
        }


        public static int ToIntOrDefault(this object data, int defaultVal = 0)
        {
            if (data == null) return defaultVal;
            var result = int.TryParse(data.ToString(), out var parseIntValue);
            return result ? parseIntValue : defaultVal;
        }


        public static int ToIntOrDefault(this DataRow row, string columnName, int defaultVal = 0)
        {
            if (row == null) return defaultVal;
            if (row.IsNull(columnName))
                int.TryParse(row[columnName].ToString(), out defaultVal);

            return defaultVal;
        }


        public static int ToIntOrDefault(this DataRow row, int columnIndex, int defaultVal = 0)
        {
            if (row == null) return defaultVal;
            if (row.IsNull(columnIndex))
                int.TryParse(row[columnIndex].ToString(), out defaultVal);

            return defaultVal;
        }

        public static short ToShortOrDefault(this object data, short defaultVal)
        {
            if (data == null) return defaultVal;
            var result = short.TryParse(data.ToString(), out var parseIntValue);
            return result ? parseIntValue : defaultVal;
        }


        public static string ToString<T>(this T[] array, string delimiter)
        {
            var data = Array.ConvertAll(array, n => n.ToString());
            return string.Join(delimiter, data);
        }


        public static T ToStringBase<T>(this string data)
        {
            var result = default(T);

            if (string.IsNullOrEmpty(data)) return result;
            var convert = TypeDescriptor.GetConverter(typeof(T));
            result = (T)convert.ConvertFrom(data);

            return result;
        }


        public static string ToStringOrDefault(this object data, string defaultVal)
        {
            return data == null ? defaultVal : data.ToString();
        }


        public static string ToStringOrDefault(this object data)
        {
            return ToStringOrDefault(data, string.Empty);
        }


        public static string ToStringOrDefault(this DataRow row, string columnName, string defaultVal)
        {
            if (row != null) defaultVal = row.IsNull(columnName) ? defaultVal : row[columnName].ToString();

            return defaultVal;
        }


        public static string ToStringOrDefault(this DataRow row, int columnIndex, string defaultVal)
        {
            if (row != null) defaultVal = row.IsNull(columnIndex) ? defaultVal : row[columnIndex].ToString().Trim();

            return defaultVal;
        }


        public static ushort ToUShortOrDefault(this object data, ushort defaultVal)
        {
            if (data == null) return defaultVal;
            var result = ushort.TryParse(data.ToString(), out var parseUShortValue);
            return result ? parseUShortValue : defaultVal;
        }


        public static Guid ToGuidOrDefault(this string data)
        {
            return ToGuidOrDefault(data, Guid.Empty);
        }


        public static Guid ToGuidOrDefault(this string data, Guid defaultVal)
        {
            return Guid.TryParse(data, out var result) ? result : defaultVal;
        }

        #endregion Methods
    }
}