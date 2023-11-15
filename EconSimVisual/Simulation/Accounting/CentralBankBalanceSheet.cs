using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Accounting
{
    [Serializable]
    internal class CentralBankBalanceSheet : IBalanceSheet
    {
        // Constructors
        public CentralBankBalanceSheet(CentralBank bank)
        {
            Bank = bank;
        }
        private CentralBank Bank { get; }

        // Summary
        public double TotalAssets => Securities + OtherAssets + Loans;
        public double TotalLiabilities => Deposits + CashEquivalents;
        public double TotalEquity => TotalAssets - TotalLiabilities;

        // Assets
        public double Securities => Bank.OwnedAssets.Where(o => o is Security).Sum(o => o.Value);
        public double OtherAssets => Bank.OwnedAssets.Where(o => !(o is Security)).Sum(o => o.Value);
        public double Loans => Bank.Loans.MadeLoans.Sum(o => o.Principal);

        // Liabilities
        public double Deposits => Bank.Deposits.Total;
        public double CashEquivalents => Bank.Town.Economy.TotalCash;
    }
}
