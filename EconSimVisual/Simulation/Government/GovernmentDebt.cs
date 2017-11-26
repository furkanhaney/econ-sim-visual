using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Government
{
    internal class GovernmentDebt : Entity
    {
        public GovernmentDebt(Government government)
        {
            IssuedBonds = new List<Bond>();
            CurrentBond = new Bond
            {
                Issuer = government,
                IsIssued = false,
                Count = 0,
                FaceValue = 1000,
                UnitPrice = 950,
                MaturityDays = 30
            };
        }

        public double TotalAmount => IssuedBonds.Sum(o => o.Count * o.FaceValue);

        public List<Bond> IssuedBonds { get; }

        public Bond CurrentBond { get; }

        public void Adjust()
        {
            Town.Trade.BondExchange.AllBonds.RemoveAll(o => o.Issuer == Government && !o.IsIssued);
            Town.Trade.BondExchange.AllBonds.Add((Bond)CurrentBond.Clone());

            foreach (var bond in IssuedBonds.ToList())
            {
                bond.MaturityDays--;
                if (bond.MaturityDays == 0)
                    bond.Mature();
            }
        }
    }
}
