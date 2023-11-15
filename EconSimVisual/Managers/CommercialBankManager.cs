using System;
using System.Linq;
using EconSimVisual.Managers.Helpers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;

namespace EconSimVisual.Managers
{
    [Serializable]
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
            ManageLimits();
        }

        private void ManageFunds()
        {
            if (Bank.Cash > 0)
                Bank.DepositCash(Bank.Cash);
            var centralBank = Bank.CentralBank;
            if (Bank.Reserves > MinReserves * 2.0)
            {
                var diff = Bank.Reserves - MinReserves * 2.0;
                new SecurityManager(Bank).BuyBondFunds(diff);
            }
            else if (Bank.Reserves > MinReserves * 1.5)
            {
                /*var diff = 0.8 * (Bank.Reserves - MinReserves * 1.5);
                var rate = centralBank.Loans.InterestRate;
                if (rate < 0.05)
                    if (centralBank.Loans.Apply(Bank, diff, 180))
                        centralBank.Loans.Take(Bank, diff, 180);*/
            }
            else
            {
                var diff = Bank.Reserves - MinReserves * 1.5;
                //new SecurityManager(Bank).SellBonds(diff);
            }
        }

        private double NonborrowedReserves => Bank.Reserves - Bank.TakenLoans.Sum(o => o.Principal);
        private double MinReserves => Bank.Deposits.Total * CentralBank.RequiredReserveRatio;
        private double TargetMinReserves => MinReserves * 1.25;
        private double TargetMaxReserves => MinReserves * 1.50;

        private void ManageLimits()
        {
            foreach (var account in Bank.Deposits.Accounts.Values)
                account.CreditLimit = 0;
        }

        private void ManageRates()
        {
            foreach (var account in Bank.Deposits.Accounts.Values.ToList())
            {
                account.SavingsRate = 0.02;
            }
        }
    }
}
