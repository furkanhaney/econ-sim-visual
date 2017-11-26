using System.Collections.Generic;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Agents
{
    internal class CentralBank : Agent, IBank
    {
        public CentralBank()
        {
            TargetCash = 100000;
        }

        protected override string DefaultName => "CentralBank";
        public Dictionary<Agent, BankAccount> Accounts { get; } = new Dictionary<Agent, BankAccount>();
        public double ReserveRatio { get; private set; } = 0.2;

        public void OpenAccount(Agent agent)
        {
            Assert(agent is CommercialBank);

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
            Assert(Accounts[agent].Balance == 0);
            Assert(HasAccount(agent));

            agent.BankAccounts.Remove(Accounts[agent]);
            Accounts.Remove(agent);
        }

        public bool HasAccount(Agent agent)
        {
            return Accounts.ContainsKey(agent);
        }

        public override void ManageFinances()
        {

        }
    }
}