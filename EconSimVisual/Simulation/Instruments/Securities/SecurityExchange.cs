using System;
using System.Collections.Generic;
using System.Linq;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    [Serializable]
    internal class SecurityExchange<T> where T : Security
    {
        public SecurityExchange()
        {
            All = new List<T>();
        }

        public List<T> PrimaryMarket => All.Where(o => !o.IsIssued).ToList();
        public List<T> SecondaryMarket => All.Where(o => o.IsIssued).ToList();
        public List<T> All { get; }
        public int ForSaleCount => All.Sum(o => o.Count);
    }
}
