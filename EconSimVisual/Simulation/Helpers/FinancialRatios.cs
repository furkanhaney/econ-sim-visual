using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Helpers
{
    internal class FinancialRatios
    {
        public FinancialRatios(Business business)
        {
            Business = business;
        }

        private Business Business { get; }
        private IBalanceSheet BalanceSheet => Business.BalanceSheet;

        public double ReturnOnEquity => Business.Profits / BalanceSheet.TotalEquity;
        public double ReturnOnAssets => Business.Profits / BalanceSheet.TotalAssets;
        public double DebtEquityRatio => BalanceSheet.TotalLiabilities / BalanceSheet.TotalEquity;
        //public double EarningsPerShare => (Business.Profits - Business.Owners.TotalDividends) / Business.Owners.OutstandingShares;
    }
}
