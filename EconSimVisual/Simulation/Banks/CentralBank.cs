using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Loans;

namespace EconSimVisual.Simulation.Banks
{
    internal class CentralBank : Agent, IBank, ILender
    {
        public CentralBank()
        {
            BalanceSheet = new CentralBankBalanceSheet(this);
            Loans = new Loans(this);
        }

        public void Tick()
        {
            Loans.Tick();
        }

        public Loans Loans { get; set; }
        protected override string DefaultName => "CentralBank";
        public Dictionary<Agent, BankAccount> Accounts { get; } = new Dictionary<Agent, BankAccount>();
        public double Deposits => Accounts.Values.GetPositives().Sum(o => o.Balance);
        public double Loans1 => -Accounts.Values.GetNegatives().Sum(o => o.Balance);
        public double ReserveRatio { get; set; } = 0.2;

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

        public override bool CanPay(double amount) => true;
        public override bool CanPayCash(double amount) => true;
        public override bool CanPayCredit(double amount) => true;
        public override void Pay(Agent payee, double amount)
        {
            PayCash(payee, amount);
        }
        public override void PayCredit(Agent payee, double amount)
        {
            throw new Exception();
        }
    }
}