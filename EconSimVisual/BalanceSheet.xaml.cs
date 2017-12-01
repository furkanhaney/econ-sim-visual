using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

namespace EconSimVisual
{
    /// <summary>
    /// Interaction logic for BalanceSheet.xaml
    /// </summary>
    internal partial class BalanceSheet : IPanel
    {
        public Simulation.Accounting.BalanceSheet Sheet { get; set; }

        public BalanceSheet()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (Sheet == null)
                return;
            LblCashEquivalents.Content = Sheet.CashEquivalents.FormatMoney();
            LblSecurities.Content = Sheet.Securities.FormatMoney();
            LblOtherAssets.Content = Sheet.OtherAssets.FormatMoney();
            LblInventory.Content = Sheet.Inventory.FormatMoney();
            LblEquipment.Content = Sheet.Equipment.FormatMoney();
            LblLoans.Content = Sheet.Loans.FormatMoney();
            LblTotalAssets.Content = Sheet.TotalAssets.FormatMoney();

            LblDebt.Content = Sheet.Debt.FormatMoney();
            LblBonds.Content = Sheet.Bonds.FormatMoney();
            LblDeposits.Content = Sheet.Deposits.FormatMoney();
            LblTotalLiabilities.Content = Sheet.TotalLiabilities.FormatMoney();
            LblTotalEquity.Content = Sheet.TotalEquity.FormatMoney();
        }

        public void Initialize()
        {

        }
    }
}
