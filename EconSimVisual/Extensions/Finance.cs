using System;

namespace EconSimVisual.Extensions
{
    internal static class Finance
    {
        public const double DailyToAnnual = 365.0, AnnualToDaily = 1.0 / 365;

        public static double GetYield(double face, double price, int days)
        {
            return Math.Pow(face / price, 365.0 / days) - 1;
        }

        public static double GetPrice(double face, double yield, int days)
        {
            return face / Math.Pow(1 + yield, days / 365.0);
        }

        public static double ConvertRate(double currentRate, double timeRatio)
        {
            return Math.Pow(1 + currentRate, timeRatio) - 1;
        }
    }
}
