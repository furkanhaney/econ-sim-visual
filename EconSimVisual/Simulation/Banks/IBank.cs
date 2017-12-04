using System.Collections.Generic;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Banks
{
    internal interface IBank
    {
        Dictionary<Agent, BankAccount> Accounts { get; }

        double Deposits { get; }
        double Loans1 { get; }

        void OpenAccount(Agent agent);
        bool HasAccount(Agent agent);
    }
}
