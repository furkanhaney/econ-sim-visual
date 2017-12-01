using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Polities;
using EconSimVisual.Simulation.Securities.Exchanges;

namespace EconSimVisual.Simulation.Securities
{
    internal class Bonds
    {
        public Bonds(Agent agent)
        {
            Agent = agent;
            Issued = new List<Bond>();
            Current = new Bond
            {
                Issuer = agent,
                IsIssued = false,
                Count = 0,
                FaceValue = 1000,
                UnitPrice = 950,
                MaturityDays = 30
            };
        }

        public Agent Agent { get; }
        public BondExchange Exchange { get; set; }
        public List<Bond> Issued { get; }
        public Bond Current { get; }
        public double TotalAmount => Issued.Sum(o => o.Count * o.FaceValue);

        public void Adjust()
        {
            if (Exchange == null)
                return;

            Exchange.AllBonds.RemoveAll(o => o.Issuer == Agent && !o.IsIssued);
            Exchange.AllBonds.Add((Bond)Current.Clone());

            foreach (var bond in Issued.ToList())
            {
                bond.MaturityDays--;
                if (bond.MaturityDays == 0)
                    bond.Mature();
            }
        }
    }
}
