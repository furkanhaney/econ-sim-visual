using System;

namespace EconSimVisual.Extensions
{
    internal static class Finance
    {
        public static double GetYield(double face, double price, int days)
        {
            return Math.Pow(face / price, 365.0 / days) - 1;
        }

        public static double GetPrice(double face, double yield, int days)
        {
            return face / Math.Pow(1 + yield, days / 365.0);
        }
    }
}
