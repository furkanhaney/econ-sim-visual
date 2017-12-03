using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Managers.Helpers;
using MoreLinq;

namespace EconSimVisual.Simulation.Managers
{
    using Agents;

    internal class CommercialBankManager : Manager
    {
        public CommercialBankManager(CommercialBank bank)
        {
            Bank = bank;
        }

        private CommercialBank Bank { get; }

        public override void Manage()
        {
            ManageRates();
            ManageFunds();
            ManageBonds();
        }

        private void ManageFunds()
        {
            if (Bank.Cash > 0)
                Bank.DepositCash(Bank.Cash);
        }

        private double MinReserves => Bank.Deposits * Town.Agents.CentralBank.ReserveRatio;
        private double TargetMinReserves => MinReserves * 1.5;
        private double TargetMaxReserves => MinReserves * 2.0;

        private void ManageBonds()
        {
            var targetAvg = (TargetMinReserves + TargetMaxReserves) / 2;
            var amount = Math.Abs(Bank.Reserves - targetAvg);
            amount = Math.Min(amount, 20000);

            var manager = new BondManager(Bank);
            if (Bank.Reserves > TargetMaxReserves)
                manager.BuyBonds(amount);
            else if (Bank.Reserves < TargetMinReserves)
                manager.SellBonds(amount);
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
