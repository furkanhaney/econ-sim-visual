using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Banks;
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

        private double MinReserves => Bank.Deposits * Town.Agents.CentralBank.ReserveRatio;
        private double TargetMinReserves => Math.Max(targetCash, MinReserves * 1.5);
        private double TargetMaxReserves => Math.Max(targetCash, MinReserves * 2.0);

        private void ManageBonds()
        {
            var targetAvg = (TargetMinReserves + TargetMaxReserves) / 2;
            var amount = Math.Abs(Bank.Reserves - targetAvg);
            amount = Math.Min(amount, 20000);

            RemoveCurrentBonds();
            if (Bank.Reserves > TargetMaxReserves)
                BuyBonds(amount);
            else if (Bank.Reserves < TargetMinReserves)
                SellBonds(amount);
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
            foreach (var account in Bank.Accounts.Values.ToList())
            {
                account.SavingsRate = 0.02;
                account.CreditRate = 0.08;
            }
        }
    }
}
