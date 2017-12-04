using System;
using System.Linq;
using EconSimVisual.Managers.Helpers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;

namespace EconSimVisual.Managers
{
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
            ManageLimits();
        }

        private void ManageFunds()
        {
            var centralBank = Bank.Town.Agents.CentralBank;
            if (Bank.Cash > 0)
                Bank.DepositCash(Bank.Cash);
            if (Bank.Reserves < TargetMinReserves)
            {
                var diff = TargetMinReserves - Bank.Reserves; // TODO: Make this non-borrowed reserves
                if (centralBank.Loans.Apply(Bank, diff, 30)) // The time required to sort out reserves
                    centralBank.Loans.Take(Bank, diff, 30);
            }
        }

        private double NonborrowedReserves => Bank.Reserves - Bank.TakenLoans.Sum(o => o.Principal);
        private double MinReserves => Bank.Deposits * Town.Agents.CentralBank.ReserveRatio;
        private double TargetMinReserves => MinReserves * 1.5;
        private double TargetMaxReserves => MinReserves * 2.0;

        private void ManageLimits()
        {
            foreach (var account in Bank.Accounts.Values)
                if (account.Owner is Business)
                    account.CreditLimit = 100000;
        }

        private void ManageBonds()
        {
            var targetAvg = (TargetMinReserves + TargetMaxReserves) / 2;
            var amount = Math.Abs(NonborrowedReserves - targetAvg);

            if (Bank.Reserves > TargetMaxReserves)
                new BondManager(Bank).BuyBonds(amount);
            else if (Bank.Reserves < TargetMinReserves)
                new BondManager(Bank).SellBonds(amount);
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
