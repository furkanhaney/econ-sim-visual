using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Accounting
{
    [Serializable]
    internal class IncomeStatement
    {
        // Summary
        public double Revenues => Sales + InterestRevenue;
        public double Expenses => Wages + Inventory + BadDebt + InterestExpense;
        public double GrossProfit { get; set; }
        public double NetProfit => Revenues - Expenses;
        public double AfterTaxProfit { get; set; }

        // Revenues
        public double Sales { get; set; }
        public double InterestRevenue { get; set; }

        // Expenses
        public double Wages { get; set; }
        public double Inventory { get; set; }
        public double BadDebt { get; set; }
        public double InterestExpense { get; set; }
    }
}
