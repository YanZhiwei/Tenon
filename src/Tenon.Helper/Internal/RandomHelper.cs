using System;
using System.Drawing;
using System.Text;

namespace Tenon.Helper.Internal
{
    public static class RandomHelper
    {
        private static readonly Random RandomSeed;


        static RandomHelper()
        {
            RandomSeed = new Random((int)DateTime.Now.Ticks);
        }


        public static string NextString(int size, bool lowerCase)
        {
            var builder = new StringBuilder(size);
            var startChar = lowerCase ? 97 : 65; //65 = A / 97 = a

            for (var i = 0; i < size; i++) builder.Append((char)(26 * RandomSeed.NextDouble() + startChar));

            return builder.ToString();
        }


        public static string NextString(string randomString, int size, bool lowerCase)
        {
            var nextString = string.Empty;

            if (!string.IsNullOrEmpty(randomString))
            {
                var builder = new StringBuilder(size);
                var maxCount = randomString.Length - 1;

                for (var i = 0; i < size; i++)
                {
                    var number = RandomSeed.Next(0, maxCount);
                    builder.Append(randomString[number]);
                }

                nextString = builder.ToString();
            }

            return lowerCase ? nextString.ToLower() : nextString.ToUpper();
        }

        public static bool NextBool()
        {
            return RandomSeed.NextDouble() >= 0.5;
        }


        public static Color NextColor()
        {
            return Color.FromArgb((byte)RandomSeed.Next(255), (byte)RandomSeed.Next(255),
                (byte)RandomSeed.Next(255));
        }


        public static DateTime NextDateTime()
        {
            int year = RandomSeed.Next(1900, DateTime.Now.Year),
                month = RandomSeed.Next(1, 12),
                day = RandomSeed.Next(1, DateTime.DaysInMonth(year, month)),
                hour = RandomSeed.Next(0, 23),
                minute = RandomSeed.Next(0, 59),
                second = RandomSeed.Next(0, 59);
            var dateTimeString = $"{year}-{month}-{day} {hour}:{minute}:{second}";
            return Convert.ToDateTime(dateTimeString);
        }

        public static double NextDouble(double miniDouble, double maxiDouble)
        {
            return RandomSeed.NextDouble() * (maxiDouble - miniDouble) + miniDouble;
        }


        public static string NextHexString(ushort byteLength)
        {
            var buffer = new byte[byteLength];
            RandomSeed.NextBytes(buffer);
            var builder = new StringBuilder();

            foreach (var x in buffer) builder.Append(x.ToString("X2"));

            return builder.ToString();
        }


        public static string NextMacAddress()
        {
            int minValue = 0, maxValue = 16;
            return
                $"{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}:{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}:{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}:{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}:{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}:{RandomSeed.Next(minValue, maxValue):x}{RandomSeed.Next(minValue, maxValue):x}"
                    .ToUpper();
        }


        public static int NextNumber(int low, int high)
        {
            return RandomSeed.Next(low, high);
        }

        public static DateTime NextTime()
        {
            var hour = RandomSeed.Next(0, 23);
            var minute = RandomSeed.Next(0, 60);
            var second = RandomSeed.Next(0, 60);
            var dateTimeString = $"{DateTime.Now:yyyy-MM-dd} {hour}:{minute}:{second}";
            return Convert.ToDateTime(dateTimeString);
        }
    }
}