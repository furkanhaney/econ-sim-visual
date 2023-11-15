using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Polities;
using Xceed.Wpf.Toolkit;

namespace EconSimVisual.Panels
{
    using Simulation.Agents;

    /// <summary>
    /// Interaction logic for CentralBankPanel.xaml
    /// </summary>
    public partial class CentralBankPanel : IPanel
    {
        private PolityAgents Agents => SimulationScreen.Polity.Agents;

        public CentralBankPanel()
        {
            InitializeComponent();
        }

        private static CentralBank CentralBank => SimulationScreen.Polity.Agents.CentralBank;

        public void Update()
        {
            GridBankReserves.SetData(CentralBank.Deposits.Accounts.Values);
            GridLoans.SetData(CentralBank.Loans.MadeLoans);
            LblCash.Content = "Cash: " + SimulationScreen.Polity.Economy.TotalCash.FormatMoney();
            LblDeposits.Content = "Deposits: " + "0";
            LblMoney.Content = "Money: " + SimulationScreen.Polity.Economy.MoneySupply.FormatMoney();
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            PieChartCash.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.NonBankBusinesses.Sum(o => o.Cash)},
                new PieChartPoint{Name="Banks", Value = Agents.Banks.Sum(o => o.Cash)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.Cash)},
                new PieChartPoint{Name="Government", Value=Agents.Government.Cash}
            });
            PieChartDeposits.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.NonBankBusinesses.Sum(o => o.CheckingBalance)},
                new PieChartPoint{Name="Banks", Value = Agents.Banks.Sum(o => o.CheckingBalance)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.CheckingBalance)},
                new PieChartPoint{Name="Government", Value=Agents.Government.CheckingBalance}
            });
            PieChartMoney.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.NonBankBusinesses.Sum(o => o.Money)},
                new PieChartPoint{Name="Banks", Value = Agents.Banks.Sum(o => o.Money)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.Money)},
                new PieChartPoint{Name="Government", Value=Agents.Government.Money}
            });
        }

        public void Initialize()
        {

        }

        private void DoubleUpDown_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var upDown = (DoubleUpDown)sender;
            if (upDown.Value is null)
                return;
            foreach (var account in Agents.CentralBank.Deposits.Accounts.Values)
                if (account.Owner is CommercialBank)
                    account.SavingsRate = (double)upDown.Value;
        }

        private void DoubleUpDownReserveRequirements_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var upDown = (DoubleUpDown)sender;
            if (upDown.Value is null)
                return;
            Agents.CentralBank.RequiredReserveRatio = (double)upDown.Value;
        }

        private void DoubleUpDownReserveRate_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var upDown = (DoubleUpDown)sender;
            if (upDown.Value is null)
                return;
            CentralBank.Deposits.InterestRate = (double)upDown.Value;
        }

        private void DoubleUpDownDiscountRate_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var upDown = (DoubleUpDown)sender;
            if (upDown.Value is null)
                return;
            CentralBank.Loans.InterestRate = (double)upDown.Value;
        }
    }
}
