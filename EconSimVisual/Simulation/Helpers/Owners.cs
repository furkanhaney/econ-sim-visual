using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Helpers
{
    internal class Owners
    {
        public static void Test()
        {

        }
        public Owners(Business business)
        {
            Business = business;
        }

        public Business Business { get; }
        public double DividendAmount { get; set; }
        public List<Stock> IssuedStocks { get; set; } = new List<Stock>();
        public int OutstandingShares => IssuedStocks.Sum(o => o.Count);
        public double BookValuePerShare => Business.NetWorth / OutstandingShares;

        public void PayDividends()
        {
            if (DividendAmount == 0)
                return;
            foreach (var stock in IssuedStocks)
                stock.PayDividends();
        }
    }
}
