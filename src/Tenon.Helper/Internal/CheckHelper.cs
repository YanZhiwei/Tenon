using System;

namespace Tenon.Helper.Internal
{
    /// <summary>
    ///     检查 帮助类
    /// </summary>
    public static class CheckHelper
    {
        #region Methods

        public static bool InRange(string data, int minValue, int maxValue)
        {
            var result = false;

            if (int.TryParse(data, out var number)) result = number >= minValue && number <= maxValue;

            return result;
        }


        public static bool InRange(DateTime date, DateTime startTime, DateTime endTime, bool includeEq)
        {
            var result = false;

            if (includeEq)
            {
                if (date >= startTime && date <= endTime) result = true;
            }
            else
            {
                if (date > startTime && date < endTime) result = true;
            }

            return result;
        }


        public static bool IsBase64(string data)
        {
            return data.Length % 4 == 0 && RegexHelper.IsMatch(data, RegexDefaults.Base64Check);
        }


        public static bool IsBigint(string value, out long number)
        {
            return long.TryParse(value, out number);
        }


        public static bool IsBinaryCodedDecimal(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.BinaryCodedDecimal);
        }

        public static bool IsBool(object data)
        {
            switch (data.ToString().Trim().ToLower())
            {
                case "0":
                    return false;

                case "1":
                    return true;

                case "是":
                    return true;

                case "否":
                    return false;

                case "yes":
                    return true;

                case "no":
                    return false;

                default:
                    return false;
            }
        }


        public static bool IsChinese(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.ChineseCheck);
        }


        public static bool IsChineseOrCharacter(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.ChineseOrCharacterCheck);
        }


        public static bool IsDate(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;

            if (RegexHelper.IsMatch(data, RegexDefaults.DateCheck))
            {
                data = data.Replace("年", "-");
                data = data.Replace("月", "-");
                data = data.Replace("日", " ");
                data = data.Replace("  ", " ");
                return DateTime.TryParse(data, out _);
            }

            return false;
        }


        public static bool IsEmail(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.EmailCheck);
        }


        public static bool IsEven(int data)
        {
            return (data & 1) == 0;
        }


        public static bool IsFilePath(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.FileCheck);
        }


        public static bool IsHexString(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.HexStringCheck);
        }


        public static bool IsIdCard(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.IdCardCheck);
        }


        public static bool IsImageFormat(byte[] data)
        {
            if (data == null || data.Length < 4) return false;

            var len = data.Length;

            try
            {
                var fileClass = data[0].ToString();
                fileClass += data[1].ToString();
                fileClass = fileClass.Trim();

                if (fileClass == "7173" || fileClass == "13780") //7173:gif;13780:PNG;
                    return true;

                var jpg = new byte[4];
                jpg[0] = 0xff;
                jpg[1] = 0xd8;
                jpg[2] = 0xff;
                jpg[3] = 0xd9;

                return data[0] == jpg[0] && data[1] == jpg[1]
                                         && data[len - 2] == jpg[2] && data[len - 1] == jpg[3];
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool IsInt(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.IntCheck);
        }


        public static bool IsIp46Address(string data)
        {
            var result = false;

            if (!string.IsNullOrEmpty(data))
            {
                var hostType = Uri.CheckHostName(data);

                if (hostType == UriHostNameType.Unknown) //譬如 "192.168.1.1:8060"或者[2001:0DB8:02de::0e13]:9010
                {
                    if (Uri.TryCreate($"http://{data}", UriKind.Absolute, out _)) result = true;
                }
                else if (hostType == UriHostNameType.IPv4 || hostType == UriHostNameType.IPv6)
                {
                    result = true;
                }
            }

            return result;
        }


        public static bool IsIp4Address(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.IpCheck);
        }


        public static bool IsLatitude(decimal data)
        {
            return !(data < -90 || data > 90);
        }


        public static bool IsLocalIp4(this string ipAddress)
        {
            var result = false;

            if (!string.IsNullOrEmpty(ipAddress) && IsIp4Address(ipAddress))
                result |= ipAddress.StartsWith("192.168.", StringComparison.OrdinalIgnoreCase) ||
                          ipAddress.StartsWith("172.", StringComparison.OrdinalIgnoreCase) ||
                          ipAddress.StartsWith("10.", StringComparison.OrdinalIgnoreCase);

            return result;
        }


        public static bool IsLongitude(decimal data)
        {
            return !(data < -180 || data > 180);
        }


        public static bool IsMacAddress(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.MacAddr6Check) ||
                   RegexHelper.IsMatch(data, RegexDefaults.MacAddr7Check);
        }


        public static bool IsNumber(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.NumberCheck);
        }


        public static bool IsOdd(int data)
        {
            return (data & 1) == 1;
        }


        public static bool IsPoseCode(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.PostCodeCheck);
        }


        public static bool IsSmallint(string value, out short number)
        {
            number = -1;
            return short.TryParse(value, out number);
        }


        public static bool IsTinyint(string value, out byte number)
        {
            number = 0;
            return byte.TryParse(value, out number);
        }


        public static bool IsUrl(string data)
        {
            return RegexHelper.IsMatch(data, RegexDefaults.UrlCheck);
        }


        public static bool IsValidPort(string port)
        {
            var result = false;
            const int minPort = 0;
            var maxPort = 65535;

            if (int.TryParse(port, out var portValue)) result = !(portValue < minPort || portValue > maxPort);

            return result;
        }


        public static bool NotNull(object data)
        {
            return data != null;
        }

        #endregion Methods
    }
}