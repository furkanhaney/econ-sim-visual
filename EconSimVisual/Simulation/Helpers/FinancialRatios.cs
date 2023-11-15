using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Helpers
{
    [Serializable]
    internal class FinancialRatios
    {
        public FinancialRatios(Business business)
        {
            Business = business;
        }

        private Business Business { get; }
        private IBalanceSheet BalanceSheet => Business.BalanceSheet;

        public double ReturnOnEquity => 0;// Business.Profits / BalanceSheet.TotalEquity;
        public double ReturnOnAssets => 0;// Business.Profits / BalanceSheet.TotalAssets;
        public double DebtEquityRatio => BalanceSheet.TotalLiabilities / BalanceSheet.TotalEquity;
        public double DebtAssetRatio => BalanceSheet.TotalLiabilities / BalanceSheet.TotalAssets;
        //public double EarningsPerShare => (Business.Profits - Business.Owners.TotalDividends) / Business.Owners.OutstandingShares;
    }
}
