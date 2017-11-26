using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Securities.Exchanges
{
    internal class BondExchange : Exchange
    {
        public BondExchange()
        {
            AllBonds = new List<Bond>();
        }

        public List<Bond> PrimaryBonds => AllBonds.Where(o => !o.IsIssued).ToList();
        public List<Bond> SecondaryBonds => AllBonds.Where(o => o.IsIssued).ToList();
        public List<Bond> AllBonds { get; set; }
        public double MaxYield => AllBonds.Count > 0 ? AllBonds.Max(o => o.Yield) : 0;
        public int ForSaleCount => AllBonds.Sum(o => o.Count);
    }
}
