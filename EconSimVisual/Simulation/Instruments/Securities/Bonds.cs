using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Instruments.Securities
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
        public SecurityExchange<Bond> Exchange { get; set; }
        public List<Bond> Issued { get; }
        public Bond Current { get; }
        public double TotalAmount => Issued.Sum(o => o.Count * o.FaceValue);
        public void Tick()
        {
            if (Exchange == null)
                return;
            Exchange.All.RemoveAll(o => o.Issuer == Agent && !o.IsIssued);
            if (Current.Count == 0)
                return;

            Exchange.All.Add((Bond)Current.Clone());
            foreach (var bond in Issued.ToList())
            {
                bond.MaturityDays--;
                if (bond.MaturityDays == 0)
                    bond.Mature();
            }
        }
    }
}
