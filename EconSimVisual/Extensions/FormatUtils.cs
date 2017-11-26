using System;
using System.Text.RegularExpressions;

namespace EconSimVisual.Extensions
{
    internal static class FormatUtils
    {
        public static string FormatMoney(this double amount)
        {
            if (amount < 0)
                return "-$" + (-amount).FormatAmount();
            return "$" + amount.FormatAmount();
        }
        public static string FormatAmount(this double amount)
        {
            if (amount >= 100000000000)
                return (amount / 1000000000).ToString("0") + "b";
            if (amount >= 10000000000)
                return (amount / 1000000000).ToString("0.0") + "b";
            if (amount >= 1000000000)
                return (amount / 1000000000).ToString("0.00") + "b";
            if (amount >= 100000000)
                return (amount / 1000000).ToString("0") + "m";
            if (amount >= 10000000)
                return (amount / 1000000).ToString("0.0") + "m";
            if (amount >= 1000000)
                return (amount / 1000000).ToString("0.00") + "m";
            if (amount >= 100000)
                return (amount / 1000).ToString("0") + "k";
            if (amount >= 10000)
                return (amount / 1000).ToString("0.0") + "k";
            if (amount >= 1000)
                return (amount / 1000).ToString("0.00") + "k";
            return amount.ToString("0.00");
        }
        public static string FormatFileSize(this double amount)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            var order = 0;
            while (amount >= 1024 && order < sizes.Length - 1)
            {
                order++;
                amount = amount / 1024;
            }
            return $"{amount:0.00} {sizes[order]}";
        }
        public static int ParseDays(this string str)
        {
            return Int32.Parse(str.Split(' ')[0]);
        }
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
        public static DateTime ToDate(int day)
        {
            return new DateTime(1900, 1, 1).AddDays(day - 1);
        }
        public static string Format(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
        public static string FormatName(this string name) => name[0] + name.ToLower().Substring(1);
    }
}
