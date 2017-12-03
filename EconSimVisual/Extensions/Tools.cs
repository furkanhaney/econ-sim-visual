using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Extensions
{
    internal static class Tools
    {
        public static double Adjust(this double number, double min, double max)
        {
            if (number < min)
                return min;
            if (number > max)
                return max;
            return number;
        }
        public static IEnumerable<BankAccount> GetPositives(this IEnumerable<BankAccount> accounts)
        {
            return accounts.Where(o => o.Balance > 0).ToList();
        }
        public static IEnumerable<BankAccount> GetNegatives(this IEnumerable<BankAccount> accounts)
        {
            return accounts.Where(o => o.Balance < 0).ToList();
        }
        public static double NextDouble(this Random random, double min, double max)
        {
            if (min > max)
                throw new Exception("Minimum must be less than maximum.");
            return random.NextDouble() * (max - min) + min;
        }
        public static double GetValue(this IEnumerable<IAsset> assets)
        {
            return assets.Sum(o => o.Value);
        }

        /*public double GetGeometricSum(double firstTerm, double rate, int count = -1)
        {
            if(count==-1)
        }*/
    }
}
