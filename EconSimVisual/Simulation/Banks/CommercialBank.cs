using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Managers;

namespace EconSimVisual.Simulation.Banks
{
    /// <summary>
    ///     Businesses that that take deposits and provide loans.
    /// </summary>
    internal class CommercialBank : Business, IBank
    {
        public CommercialBank()
        {
            Manager = new CommercialBankManager(this);
        }

        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "IBank" + id;
        public double InterestRate { get; set; } = 0.001;
        public double RateSpread { get; set; } = 3;
        public double MinimumPaymentConstant { get; set; } = 20;
        public double MinimumPaymentRate { get; set; } = 0.2;
        public double SavingsRate { get; set; }
        public double CreditRate { get; set; }
        public double ReserveRatio => Deposits == 0 ? 0 : Reserves / Deposits;
        public double RequiredReserves => Deposits * Town.Agents.CentralBank.ReserveRatio;
        public double Reserves => Cash + BankAccounts.Sum(o => o.Balance);
        public double Deposits => Accounts.Values.GetPositives().Sum(o => o.Balance);
        public double Loans => -Accounts.Values.GetNegatives().Sum(o => o.Balance);
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

        public void CloseAccount(Agent agent)
        {
            Assert(HasAccount(agent));
            Assert(Accounts[agent].Balance == 0);

            agent.BankAccounts.Remove(Accounts[agent]);
            Accounts.Remove(agent);
        }

        public bool HasAccount(Agent agent)
        {
            return Accounts.ContainsKey(agent);
        }

        public override void FirstTick()
        {
            Manager.Manage();
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
                    InterestExpenses += account.Balance * SavingsRate;
                    account.Balance *= 1 + Finance.ConvertRate(SavingsRate, Finance.AnnualToDaily);
                }
                else
                {
                    InterestRevenues += -account.Balance * CreditRate;
                    account.Balance *= 1 + Finance.ConvertRate(CreditRate, Finance.AnnualToDaily);
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