using System;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Instruments.Loans;
using EconSimVisual.Simulation.Instruments.Securities;

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
            BalanceSheet = new Accounting.BalanceSheet(this);
            OwnedAssets = new List<IAsset>();
            BankAccounts = new List<BankAccount>();
            Goods = CollectionsExtensions.InitializeDictionary<Good>();
            MadeLoans = new List<Loan>();
            TakenLoans = new List<Loan>();
        }

        public IBalanceSheet BalanceSheet { get; protected set; }
        public IList<IAsset> OwnedAssets { get; }
        public IEnumerable<Security> OwnedSecurities => OwnedAssets.Where(o => o is Security).Cast<Security>();
        public IEnumerable<Bond> OwnedBonds => OwnedAssets.Where(o => o is Bond).Cast<Bond>();
        public IList<BankAccount> BankAccounts { get; }
        public IDictionary<Good, double> Goods { get; }
        public double Cash { get; set; }
        public double Money => Cash + CheckingBalance;
        public double CheckingBalance => BankAccounts.GetPositives().Sum(o => o.Balance);
        public double NetMoney => Cash + BankAccounts.Sum(o => o.Balance);

        public List<Loan> MadeLoans { get; }
        public List<Loan> TakenLoans { get; }

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
                {
                    account.Pay(payee, amount);
                    return;
                }
        }
        public virtual void FirstTick()
        {

        }
        public virtual void LastTick()
        {

        }

        public void Transfer(Agent to, Good good, double amount)
        {
            Goods[good] -= amount;
            to.Goods[good] += amount;
            Debug.Assert(Goods[good] >= 0);
        }

        public void DepositCash(double amount)
        {
            BankAccounts.MaxBy(o => GetDepositGain(o, amount)).Deposit(amount);
        }

        public void WithdrawCash(double amount)
        {
            foreach (var account in BankAccounts.OrderBy(o => GetWithdrawalLoss(o, amount)))
                if (account.AvailableCredit >= amount)
                {
                    account.Withdraw(amount);
                    return;
                }
        }

        public static double GetWithdrawalLoss(BankAccount account, double amount)
        {
            if (account.Balance >= amount)
                return account.SavingsRate * amount;
            if (account.Balance > 0)
                return account.SavingsRate * account.Balance + account.CreditRate * (account.Balance - amount);
            return account.CreditRate * amount;
        }

        public static double GetDepositGain(BankAccount account, double amount)
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