using System;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Managers.Helpers
{
    // Buys and sells securities to keep their total value in given range
    internal class BondManager
    {
        private Agent Agent { get; }
        public SecurityExchange<Bond> BondExchange => Agent.Town.Trade.BondExchange;

        public BondManager(Agent agent)
        {
            Agent = agent;
        }

        public void SellBonds(double amount)
        {
            RemoveCurrentBonds();
            var maxYield = BondExchange.All.Count == 0 ? 0 : BondExchange.All.Max(o => o.Yield);
            var targetYield = Math.Max(maxYield * 1.001, 0.0001);
            var leftToSell = amount;
            foreach (var bond in Agent.OwnedBonds)
            {
                var price = Finance.GetPrice(bond.FaceValue, targetYield, bond.MaturityDays);
                var count = Math.Min(bond.Count, (int)(leftToSell / price));
                if (count == 0)
                    continue;
                bond.UnitPrice = price;
                bond.OnSaleCount = count;
                BondExchange.All.Add(bond);
            }
        }

        public void BuyBonds(double amount)
        {
            RemoveCurrentBonds();
            IBondEvaluator evaluator = new BondEvaluator();
            var leftToBuy = amount;
            var bonds = BondExchange.All.OrderByDescending(o => evaluator.GetAdjustedYield(o)).ToList();
            foreach (var current in bonds)
            {
                var count = Math.Min(current.Count, (int)(leftToBuy / current.UnitPrice));
                if (count == 0)
                    continue;
                Purchase(current, count);
                leftToBuy -= current.UnitPrice * count;
            }
        }

        private void RemoveCurrentBonds()
        {
            BondExchange.All.RemoveAll(o => o.Owner == Agent);
        }

        private void Purchase(Security security, int count)
        {
            var purchase = new SecurityPurchase
            {
                Security = security,
                NewOwner = Agent,
                Count = count
            };
            purchase.Execute();
        }
    }
}
