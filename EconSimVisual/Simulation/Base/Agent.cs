using System;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Banks;
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
        protected Agent()
        {
            BalanceSheet = new BalanceSheet(this);
            OwnedAssets = new List<IAsset>();
            BankAccounts = new List<BankAccount>();
            Goods = CollectionsExtensions.InitializeDictionary<Good>();
        }

        public IBalanceSheet BalanceSheet { get; protected set; }
        public IList<IAsset> OwnedAssets { get; }
        public IList<BankAccount> BankAccounts { get; }
        public IDictionary<Good, double> Goods { get; }
        public double Cash { get; set; }
        public double Money => Cash + CheckingBalance;
        public double CheckingBalance => BankAccounts.GetPositives().Sum(o => o.Balance);
        public double TargetCash { get; set; }
        public virtual bool CanPay(double amount) => CanPayCash(amount) || CanPayCredit(amount);
        public virtual bool CanPayCash(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            return Cash >= amount;
        }
        public virtual bool CanPayCredit(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            return BankAccounts.Count > 0 && BankAccounts.Max(o => o.AvailableCredit) >= amount;
        }
        public virtual void Pay(Agent payee, double amount)
        {
            if (!CanPay(amount))
                throw new Exception(this + " has " + Money.FormatMoney() + " and cannot pay " + amount.FormatMoney() + ".");
            if (Cash >= amount)
                PayCash(payee, amount);
            else
                PayCredit(payee, amount);
        }
        public virtual void PayCash(Agent payee, double amount)
        {
            Cash -= amount;
            payee.Cash += amount;
        }
        public virtual void PayCredit(Agent payee, double amount)
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