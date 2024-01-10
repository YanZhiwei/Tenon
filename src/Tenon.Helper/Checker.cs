using System;
using System.IO;
using System.Text.RegularExpressions;
using Tenon.Extensions.Collection;
using Tenon.Extensions.Models;
using Tenon.Extensions.System;
using Tenon.Helper.Internal;

namespace Tenon.Helper
{
    public static class Checker
    {
        #region Methods

        public static Validation Begin()
        {
            return null;
        }

        public static Validation Check(this Validation validation, Func<bool> checkFactory, string pattern,
            string argumentName)
        {
            return Check<ArgumentException>(validation, checkFactory,
                $"The parameter {argumentName} is not valid.");
        }

        public static Validation Check<TException>(this Validation validation, Func<bool> checkedFactory,
            string message)
            where TException : Exception
        {
            if (checkedFactory())
                return validation ?? new Validation
                {
                    IsValid = true
                };

            var exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }

        public static Validation CheckDirectoryExist(this Validation validation, string data)
        {
            return Check<DirectoryNotFoundException>(validation, () => Directory.Exists(data),
                $"The specified directory path {data} does not exist.");
        }


        public static Validation CheckedFileExt(this Validation validation, string actualFileExt,
            string[] expectFileExt)
        {
            var allowFileExt = expectFileExt.ToString(",");
            return Check<FileNotFoundException>(validation, () => expectFileExt.ContainIgnoreCase(actualFileExt),
                $"The file type is illegal and must be a file with the {allowFileExt} suffix.");
        }


        public static Validation CheckedFileExt(this Validation validation, string actualFileExt, string expectFileExt)
        {
            return Check<FileNotFoundException>(validation, () => actualFileExt.CompareIgnoreCase(expectFileExt),
                $"The file type is illegal and must be a file with the {expectFileExt} suffix.");
        }


        public static Validation CheckFileExists(this Validation validation, string data)
        {
            return Check<FileNotFoundException>(validation, () => File.Exists(data),
                $"The specified file path {data} does not exist.");
        }


        public static Validation InRange(this Validation validation, int data, int min, int max, string argumentName)
        {
            return Check<ArgumentOutOfRangeException>(validation, () => data >= min && data <= max,
                $"The parameter {argumentName} must be between {min} and {max}.");
        }


        public static Validation IsChinese(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsChinese(data), RegexDefaults.ChineseCheck, argumentName);
        }


        public static Validation IsEmail(this Validation validation, string email, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsEmail(email), RegexDefaults.EmailCheck, argumentName);
        }


        public static Validation IsFilePath(this Validation validation, string data)
        {
            return Check<ArgumentException>(validation, () => CheckHelper.IsFilePath(data),
                $"The specified file path {data} is illegal.");
        }

        public static Validation IsHexString(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsHexString(data), RegexDefaults.HexStringCheck, argumentName);
        }


        public static Validation IsIdCard(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsIdCard(data), RegexDefaults.IdCardCheck, argumentName);
        }


        public static Validation IsInt(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsInt(data), RegexDefaults.IntCheck, argumentName);
        }


        public static Validation IsIp(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsIp4Address(data), RegexDefaults.IpCheck, argumentName);
        }


        public static Validation IsNumber(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsNumber(data), RegexDefaults.NumberCheck, argumentName);
        }


        public static Validation IsPoseCode(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsPoseCode(data), RegexDefaults.PostCodeCheck, argumentName);
        }


        public static Validation IsUrl(this Validation validation, string data, string argumentName)
        {
            return Check(validation, () => CheckHelper.IsUrl(data), RegexDefaults.UrlCheck, argumentName);
        }


        public static Validation NotEqual(this Validation validation, object data, object equalObj, string argumentName)
        {
            return Check<ArgumentException>(validation, () => data != equalObj,
                $"The parameter {argumentName} cannot be equal to {data}.");
        }


        public static Validation NotNull(this Validation validation, object data, string argumentName)
        {
            return Check<ArgumentNullException>(validation, () => CheckHelper.NotNull(data),
                $"The parameter {argumentName} cannot be a null reference.");
        }


        public static Validation NotNullOrEmpty(this Validation validation, string input, string argumentName)
        {
            return Check<ArgumentNullException>(validation, () => !string.IsNullOrEmpty(input),
                $"The parameter {argumentName} cannot be an empty reference or an empty string.");
        }


        public static Validation RegexMatch(this Validation validation, string input, string pattern,
            string argumentName)
        {
            return Check<ArgumentException>(validation, () => Regex.IsMatch(input, pattern),
                $"The parameter {input} cannot match the format of {argumentName}.");
        }

        #endregion Methods
    }
}