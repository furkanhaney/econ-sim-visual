using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Helpers
{
    [Serializable]
    internal class Owners
    {
        public Owners(Business business)
        {
            Business = business;
        }
        private Business Business { get; }

        public double Dividends { get; set; }
        public List<Stock> IssuedStocks { get; set; } = new List<Stock>();

        public int OutstandingShares => IssuedStocks.Sum(o => o.Count);
        public double TotalDividends => Dividends * OutstandingShares;

        public void PayDividends()
        {
            if (Dividends == 0)
                return;
            foreach (var stock in IssuedStocks)
                stock.PayDividends();
        }
    }
}
