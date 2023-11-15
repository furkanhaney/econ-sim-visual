using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual
{
    /// <summary>
    /// Interaction logic for IncomeStatementView.xaml
    /// </summary>
    internal partial class IncomeStatementView : IPanel
    {
        private Business business;
        public Business Business
        {
            get => business;
            set
            {
                business = value;
                Update();
            }
        }
        private IncomeStatement Statement => Business.Income;

        public IncomeStatementView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            if (Business != null)
            {

                LblSalesRevenue.Content = Statement.Sales.FormatMoney();
                LblInterestRevenue.Content = Statement.InterestRevenue.FormatMoney();
                LblTotalRevenue.Content = Statement.Revenues.FormatMoney();

                LblWages.Content = Statement.Wages.FormatMoney();
                LblInventory.Content = Statement.Inventory.FormatMoney();
                LblBadDebt.Content = Statement.BadDebt.FormatMoney();
                LblInterestExpenses.Content = Statement.InterestExpense.FormatMoney();
                LblTotalExpenses.Content = Statement.Expenses.FormatMoney();

                LblGrossProfit.Content = Statement.GrossProfit.FormatMoney();
                LblNetProfit.Content = Statement.NetProfit.FormatMoney();
                LblAfterTaxProfit.Content = Statement.AfterTaxProfit.FormatMoney();
            }
        }
    }
}
