using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Accounting
{
    /// <summary>
    /// A pseudo-real balance sheet to track company finances
    /// </summary>
    internal class BalanceSheet : IBalanceSheet
    {
        // Constructors
        public BalanceSheet(Agent agent)
        {
            Agent = agent;
        }
        private Agent Agent { get; }

        // Summary
        public double TotalAssets => CashEquivalents + Securities + OtherAssets + Inventory + Equipment + Loans;
        public double TotalLiabilities => Debt + Deposits;
        public double TotalEquity => TotalAssets - TotalLiabilities;

        // Assets
        public double CashEquivalents => Agent.Cash + Agent.CheckingBalance;
        public double Securities => Agent.OwnedAssets.Where(o => o is Security).Sum(o => o.Value);
        public double OtherAssets => Agent.OwnedAssets.Where(o => !(o is Security)).Sum(o => o.Value);
        public double Inventory => Agent.Goods.Sum(o => o.Value * Agent.Town.Trade.GetLastPrice(o.Key));
        public double Equipment => Agent is Manufacturer m ? m.Capital * m.Town.Trade.GetLastPrice(Good.Capital) : 0;
        public double Loans => Agent is IBank bank ? bank.Loans : 0;

        // Liabilities
        public double Debt => -Agent.BankAccounts.GetNegatives().Sum(o => o.Balance);
        public double Deposits => Agent is IBank bank ? bank.Deposits : 0;
    }
}
