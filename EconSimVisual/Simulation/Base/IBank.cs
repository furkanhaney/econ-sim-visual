namespace EconSimVisual.Simulation.Base
{
    using System.Collections.Generic;

    using Helpers;

    internal interface IBank
    {
        Dictionary<Agent, BankAccount> Accounts { get; }

        void OpenAccount(Agent agent);
        void CloseAccount(Agent agent);
        bool HasAccount(Agent agent);
    }
}
