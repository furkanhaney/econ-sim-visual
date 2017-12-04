using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Managers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Banks
{
    /// <summary>
    ///     Businesses that that take deposits and provide loans.
    /// </summary>
    internal class CommercialBank : Business, IBank
    {
        public CommercialBank()
        {
        }

        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "Bank" + id;
        public double MinimumPaymentConstant { get; set; } = 20;
        public double MinimumPaymentRate { get; set; } = 0.2;
        public double ReserveRatio => Deposits == 0 ? 0 : Reserves / Deposits;
        public double RequiredReserves => Deposits * Town.Agents.CentralBank.ReserveRatio;
        public double Reserves => Cash + BankAccounts.Sum(o => o.Balance);
        public double Deposits => Accounts.Values.GetPositives().Sum(o => o.Balance);
        public double Loans1 => -Accounts.Values.GetNegatives().Sum(o => o.Balance);
        public override double Revenues => LastInterestRevenues;
        public override double Expenses => LastInterestExpenses;
        public override double Profits => Revenues - Expenses;
        public double InterestRevenues { get; set; }
        public double InterestExpenses { get; set; }
        public double LastInterestRevenues { get; set; }
        public double LastInterestExpenses { get; set; }

        public Dictionary<Agent, BankAccount> Accounts { get; } = new Dictionary<Agent, BankAccount>();

        public void OpenAccount(Agent agent)
        {
            var account = new BankAccount
            {
                Bank = this,
                Owner = agent,
                Balance = 0
            };
            Accounts.Add(agent, account);
            agent.BankAccounts.Add(account);
        }

        public bool HasAccount(Agent agent)
        {
            return Accounts.ContainsKey(agent);
        }

        public override void FirstTick()
        {
            if (IsPeriodStart)
                CollectMinimumPayments();
            ApplyRates();
            base.FirstTick();
        }

        protected override void ResetStats()
        {
            LastInterestRevenues = InterestRevenues;
            LastInterestExpenses = InterestExpenses;

            InterestRevenues = 0;
            InterestExpenses = 0;

            base.ResetStats();
        }

        protected void ApplyRates()
        {
            foreach (var account in Accounts.Values)
                if (account.Balance >= 0)
                {
                    InterestExpenses += account.Balance * account.SavingsRate;
                    account.Balance *= 1 + Finance.ConvertRate(account.SavingsRate, Finance.AnnualToDaily);
                }
                else
                {
                    InterestRevenues += -account.Balance * account.CreditRate;
                    account.Balance *= 1 + Finance.ConvertRate(account.CreditRate, Finance.AnnualToDaily);
                }
        }

        protected void CollectMinimumPayments()
        {
            foreach (var account in Accounts.Values.GetNegatives())
            {
                var minPayment = account.MinimumPayment;

                if (account.Owner.CanPayCash(minPayment))
                    account.Deposit(minPayment);
                else
                    Log(
                        account.Owner + " could not make a minimum payment of " + minPayment.FormatMoney() +
                        " to " + this + ".", LogType.NonPayment);
            }
        }
    }
}