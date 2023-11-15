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
using EconSimVisual.Simulation.Instruments.Loans;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Accounting
{
    [Serializable]
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
        private int lastCalculated = int.MinValue;
        private double cachedEquity = 0;

        public double TotalAssets => CashEquivalents + Securities + OtherAssets + Inventory + Equipment + Loans;
        public double TotalLiabilities => Debt + Deposits + Bonds;
        public double TotalEquity
        {
            get
            {
                if (lastCalculated == Entity.Day)
                    return cachedEquity;
                cachedEquity = TotalAssets - TotalLiabilities;
                lastCalculated = Entity.Day;
                return cachedEquity;
            }
        }
        

        // Assets
        public double CashEquivalents => Agent.Cash + Agent.CheckingBalance;
        public double Securities => Agent.OwnedAssets.Where(o => o is Security).Sum(o => o.Value);
        public double OtherAssets => Agent.OwnedAssets.Where(o => !(o is Security)).Sum(o => o.Value);
        public double Inventory => Agent.Goods.Sum(o => o.Value * (Agent.Town.Trade as TownTrade).GetUnitPrice(o.Key));
        public double Equipment => Agent is Manufacturer m ? m.Capital * (m.Town.Trade as TownTrade).GetUnitPrice(Good.Capital) : 0;
        public double Loans => Agent is ILender l ? l.Loans.MadeLoans.Sum(o => o.Principal) : 0;

        // Liabilities
        public double Debt => LoanDebt + CreditDebt + WageDebt;
        public double Bonds => Agent is IBondIssuer a ? a.Bonds.TotalAmount : 0;
        public double Deposits => Agent is IDepository d ? d.Deposits.Total : 0;

        // Details
        private double WageDebt => Agent is Business b ? b.Labor.UnpaidWages : 0;

        private double CreditDebt => -Agent.BankAccounts.GetNegatives().Sum(o => o.Balance);
        private double LoanDebt => Agent.TakenLoans.Sum(o => o.Payment);
    }
}
