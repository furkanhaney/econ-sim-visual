using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Securities;
using MoreLinq;

namespace EconSimVisual.Simulation.Managers
{
    using Agents;

    internal class CommercialBankManager : BusinessManager
    {
        public const double targetCash = 50000;

        public CommercialBankManager(CommercialBank bank)
        {
            Bank = bank;
        }

        private CommercialBank Bank { get; }

        public override void Manage()
        {
            ManageRates();
            AdjustFunds();
        }

        private void AdjustFunds()
        {
            Bank.TargetCash = targetCash;
            ManageBonds();
        }

        private void ManageBonds()
        {
            var minReserves = Bank.Deposits * Town.Agents.CentralBank.ReserveRatio;
            var targetMin = Math.Max(targetCash, minReserves * 1.5);
            var targetMax = Math.Max(targetCash, minReserves * 2.0);
            var targetAvg = (targetMin + targetMax) / 2;

            RemoveCurrentBonds();
            if (Bank.Reserves > targetMax)
                BuyBonds(Bank.Reserves - targetAvg);
            else if (Bank.Reserves < targetMin)
                SellBonds(targetAvg - Bank.Reserves);
        }

        private void SellBonds(double amount)
        {
            var exchange = Town.Trade.BondExchange;
            var targetYield = Math.Max(exchange.MaxYield * 1.001, 0.0001);
            double putOnMarket = 0;
            foreach (Bond bond in Bank.OwnedAssets.Where(o => o is Bond))
            {
                var price = Finance.GetPrice(bond.FaceValue, targetYield, bond.MaturityDays);
                if (putOnMarket >= amount) // Enough on sale
                    exchange.AllBonds.Remove(bond);
                else if (bond.Count * price <= amount - putOnMarket) // Bond not too big
                {
                    bond.UnitPrice = price;
                    exchange.AllBonds.Add(bond);
                }
            }
        }

        private void BuyBonds(double amount)
        {
            var leftToBuy = amount;
            var bonds = GetBonds(leftToBuy);

            while (Town.Trade.BondExchange.ForSaleCount > 0)
            {
                var best = bonds.MaxBy(o => o.Yield);
                var purchase = new SecurityTransfer
                {
                    Security = best,
                    Count = GetCountOfBondPurchase(best, leftToBuy),
                    NewOwner = Bank
                };
                if (Bank.CanPay(purchase.TotalPrice))
                {
                    purchase.Execute();
                    bonds = GetBonds(leftToBuy);
                }
                else
                    break;
            }
        }
        private void RemoveCurrentBonds()
        {
            Town.Trade.BondExchange.AllBonds.RemoveAll(o => o.Owner == Bank);
        }
        private IEnumerable<Bond> GetBonds(double maxPrice)
        {
            return Town.Trade.BondExchange.AllBonds.Where(o => o.UnitPrice <= maxPrice);
        }

        private static int GetCountOfBondPurchase(Security bond, double leftToBuy)
        {
            if (bond.UnitPrice * bond.Count < leftToBuy)
                return bond.Count;
            return (int)(leftToBuy / bond.UnitPrice);
        }

        private void ManageRates()
        {
            if (Bank.Reserves > 0.6)
                Bank.InterestRate /= 1.02;
            if (Bank.Reserves < 0.4)
                Bank.InterestRate *= 1.02;
            if (Bank.InterestRate < 0.0001)
                Bank.InterestRate = 0.0001;
            if (Bank.InterestRate > 0.0025)
                Bank.InterestRate = 0.0025;
            Bank.SavingsRate = 0;
            Bank.CreditRate = Bank.InterestRate * Bank.RateSpread;
        }
    }
}
