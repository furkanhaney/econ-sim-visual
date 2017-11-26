using System;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Base
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Extensions;
    using Helpers;

    using MoreLinq;

    /// <summary>
    ///     Entities that can take actions and possess resources.
    /// </summary>
    internal abstract class Agent : Entity
    {
        public IList<IAsset> OwnedAssets { get; set; } = new List<IAsset>();
        public IList<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public IDictionary<Good, double> Goods { get; set; } = CollectionsExtensions.InitializeDictionary<Good>();
        public double Cash { get; set; }
        public double InventoryValue => Goods.Sum(o => o.Value * Town.Trade.GetLastPrice(o.Key));
        public double AvailableFunds => Cash + BankAccounts.Sum(o => o.AvailableCredit);
        public double Money => Cash + BankAccounts.GetPositives().Sum(o => o.Balance);
        public double Securities => OwnedAssets.Where(o => o is Security).Sum(o => ((Security)o).Value);
        public double BaseAssets => Money + OwnedAssets.Sum(o => o.Value) + InventoryValue;
        public double BaseLiabilities => -BankAccounts.GetNegatives().Sum(o => o.Balance);
        public virtual double Assets => BaseAssets;
        public virtual double Liabilities => BaseLiabilities;
        public double NetWorth => Assets - Liabilities;
        public double TargetCash { get; set; }
        public bool CanPay(double amount) => CanPayCash(amount) || CanPayCredit(amount);
        public bool CanPayCash(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            return Cash >= amount;
        }
        public bool CanPayCredit(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            return BankAccounts.Count > 0 && BankAccounts.Max(o => o.AvailableCredit) >= amount;
        }
        public void Pay(Agent payee, double amount)
        {
            if (!CanPay(amount))
                throw new Exception(this + " has " + Money.FormatMoney() + " and cannot pay " + amount.FormatMoney() + ".");
            if (Cash >= amount)
                PayCash(payee, amount);
            else
                PayCredit(payee, amount);
        }
        public void PayCash(Agent payee, double amount)
        {
            Cash -= amount;
            payee.Cash += amount;
        }
        public void PayCredit(Agent payee, double amount)
        {
            foreach (var account in BankAccounts.OrderBy(o => GetWithdrawalLoss(o, amount)))
                if (account.AvailableCredit >= amount)
                    account.Pay(payee, amount);
        }
        public virtual void FirstTick()
        {

        }
        public virtual void LastTick()
        {
            ManageFinances();
        }
        public virtual void ManageFinances()
        {
            ManageCash();
        }
        public virtual void ManageCash()
        {
            if (Cash > TargetCash)
            {
                if (BankAccounts.Count > 0)
                    BankAccounts.MaxBy(o => GetDepositGain(o, Cash - TargetCash)).Deposit(Cash - TargetCash);
            }
            else if (Cash < TargetCash)
            {
                var accounts = BankAccounts.Where(o => o.AvailableCredit >= TargetCash - Cash).ToList();
                if (accounts.Count > 0)
                    accounts.MinBy(o => GetWithdrawalLoss(o, TargetCash - Cash)).Withdraw(TargetCash - Cash);
            }
        }
        public void Transfer(Agent to, Good good, double amount)
        {
            Goods[good] -= amount;
            to.Goods[good] += amount;
            Debug.Assert(Goods[good] >= 0);
        }

        private static double GetWithdrawalLoss(BankAccount account, double amount)
        {
            if (account.Balance >= amount)
                return account.SavingsRate * amount;
            if (account.Balance > 0)
                return account.SavingsRate * account.Balance + account.CreditRate * (account.Balance - amount);
            return account.CreditRate * amount;
        }

        private static double GetDepositGain(BankAccount account, double amount)
        {
            if (account.Balance >= 0)
                return account.SavingsRate * amount;
            var newBalance = account.Balance + amount;
            if (newBalance > 0)
                return newBalance * account.SavingsRate - account.Balance * account.CreditRate;
            return -account.Balance * account.CreditRate;
        }
    }
}