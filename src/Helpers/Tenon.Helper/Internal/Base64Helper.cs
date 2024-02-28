using System;
using System.Text;

namespace Tenon.Helper.Internal
{
    public static class Base64Helper
    {
        public static string ParseBase64String(string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }


        public static string ToBase64String(string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}