using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Managers.Helpers
{
    [Serializable]
    // Buys and sells securities to keep their total value in given range
    internal class SecurityManager
    {
        private Agent Agent { get; }
        public SecurityExchange<Bond> BondExchange => Agent.Town.Trade.BondExchange;
        public SecurityExchange<Stock> StockExchange => Agent.Town.Trade.StockExchange;

        public SecurityManager(Agent agent)
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
            var bonds = BondExchange.All.Where(o => o.Owner != Agent).OrderByDescending(o => evaluator.GetAdjustedYield(o)).ToList();
            BuySecurities(bonds, amount);
        }

        public void BuyStocks(double amount)
        {
            var stocks = StockExchange.All.Where(o => !(o.Issuer is MutualFund) && o.Owner != Agent && o.UnitPrice > 0).OrderBy(o => (o.Issuer as Business).Ratios.DebtAssetRatio).ToList();
            BuySecurities(stocks, amount);
        }

        public void BuyMutualFunds(double amount)
        {
            var funds = StockExchange.PrimaryMarket.Where(o => o.Issuer is MutualFund && o.Owner != Agent);
            BuySecurities(funds, amount);
        }

        public void BuyBondFunds(double amount)
        {
            var funds = StockExchange.PrimaryMarket.Where(o => o.Issuer is MutualFund m && m.Type == FundType.Bond && o.Owner != Agent);
            BuySecurities(funds, amount);
        }

        private void RemoveCurrentBonds()
        {
            BondExchange.All.RemoveAll(o => o.Owner == Agent);
        }

        private void BuySecurities<T>(IEnumerable<T> securities, double amount) where T : Security
        {
            var leftToBuy = amount;
            var secs = securities.Cast<Security>().ToList().Shuffle();
            foreach (var security in secs)
            {
                var count = Math.Min(security.Count, (int)(leftToBuy / security.UnitPrice));
                if (count == 0)
                    continue;
                Purchase(security, count);
                leftToBuy -= security.UnitPrice * count;
            }
        }

        private void Purchase(Security security, int count)
        {
            var purchase = new SecurityPurchase
            {
                Town = Agent.Town,
                Security = security,
                NewOwner = Agent,
                Count = count
            };
            purchase.Execute();
        }
    }
}
