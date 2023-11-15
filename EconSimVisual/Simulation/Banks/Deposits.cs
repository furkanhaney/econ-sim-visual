using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Banks
{
    [Serializable]
    internal class Deposits
    {
        public IDepository Agent { get; }
        public IDictionary<Agent, BankAccount> Accounts { get; }
        public double InterestRate
        {
            get
            {
                return (Accounts as Dictionary<Agent, BankAccount>).Values.ToList()[0].SavingsRate;
            }
            set
            {
                foreach (var account in Accounts.Values)
                    account.SavingsRate = value;
            }
        }
        public double Total => Accounts.Values.Sum(o => o.Balance);

        public double DailyInterestExpenses
        {
            get
            {
                return Accounts.Values.Sum(o => o.Balance * Finance.ConvertRate(o.SavingsRate, Finance.AnnualToDaily));
            }
        }

        public Deposits(IDepository agent)
        {
            Agent = agent;
            Accounts = new Dictionary<Agent, BankAccount>();
        }

        public void OpenAccount(Agent agent)
        {
            var account = new BankAccount
            {
                Bank = Agent,
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

        public void ApplyInterest()
        {
            foreach (var account in Accounts.Values)
                account.Balance *= 1 + Finance.ConvertRate(account.SavingsRate, Finance.AnnualToDaily);
        }
    }
}
