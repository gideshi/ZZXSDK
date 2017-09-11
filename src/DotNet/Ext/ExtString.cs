using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class ExtString
    {
        public static string Subs(this string s, int length, string houzhui)
        {
            if ((s.Length > length) && (length > 0))
            {
                s = s.Substring(0, length) + houzhui;
            }
            return s;
        }

        public static string Subs(this string s, int length)
        {
            return s.Subs(length, "...");
        }

        public static long GetLong(this string s)
        {
            long result = 0L;
            long.TryParse(s, out result);
            return result;
        }


        public static int GetInt(this string s)
        {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }

        public static decimal GetDecimal(this string s)
        {
            decimal result = 0;
            decimal.TryParse(s, out result);
            return result;
        }

        public static bool GetBool(this string s)
        {
            return s != null && !(s == "") && !(s == "0") && !(s.ToLower() == "false");
        }

        public static DateTime GetDateTime(this string s)
        {
            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(s))
            {
                DateTime.TryParse(s, out now);
            }
            if (now.ToString("yyyy-MM-dd") == "0001-01-01")
            {
                return DateTime.Now;
            }
            return now;
        }

    }


}
