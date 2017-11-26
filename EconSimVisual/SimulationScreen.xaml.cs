using System.IO;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Town;

namespace EconSimVisual
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Extensions;
    using Xceed.Wpf.AvalonDock.Controls;

    using Cursors = System.Windows.Input.Cursors;

    internal partial class SimulationScreen
    {
        public SimulationScreen()
        {
            File.WriteAllText(@"D:\logs.txt", "");
            Town = Town.Current = new Initializer().Initialize();
            InitializeGui();
            SimInitialized = true;
            Progress();
            RunTests();
        }

        private void RunTests()
        {
            BankAccount.Test();
        }

        public static bool SimInitialized { get; set; }
        public Town Town { get; }
        private IEnumerable<IPanel> AllPanels
        {
            get
            {
                return TabControlMain.Items.Cast<TabItem>()
                    .Select(o => o.FindLogicalChildren<DependencyObject>().First()).Cast<IPanel>();
            }
        }

        private void Progress()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            TechnicalPanel.StartTime = DateTime.UtcNow;
            var days = CmbSimDays.Text.ParseDays();
            ChartsPanel.ReadGoodInputs(out var good, out var prop1, out var timeFrame);
            ChartsPanel.ReadEconomicInputs(out var prop2, out var timeFrame2);
            for (var i = 0; i < days; i++)
            {
                Town.Tick();
                ChartsPanel.GoodsChartPoints.Add(Panels.ChartsPanel.GetChartPoint(good, prop1));
                ChartsPanel.EconomicsChartPoints.Add(Panels.ChartsPanel.GetChartPoint(prop2));
            }
            Update();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void BtnProgress_Click(object sender, RoutedEventArgs e)
        {
            Progress();
        }

        private void InitializeGui()
        {
            InitializeComponent();
            Background = RandomUtils.GetRandomBackgroundColor();
            foreach (var panel in AllPanels)
                panel.Initialize();
            DataContext = this;
        }

        private void Update()
        {
            foreach (var panel in AllPanels)
                panel.Update();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            var econ = Town.Economy;

            LblDay.Content = "Date: " + FormatUtils.ToDate(Town.Day).Format();
            LblPopulation.Content = "Population: " + Town.Agents.Population.Count.ToString("###,##0");
            LblGdp.Content = "GDP: " + econ.NominalGdp.FormatMoney();
            LblTreasury.Content = "Treasury: " + Town.Agents.Government.Cash.FormatMoney();
            LblJobs.Content = "Available Jobs: " + Town.JobsAvailable.Count;
            LblAvgHunger.Content = "Avg Hunger: " + econ.AverageHunger.FormatAmount();
            LblAvgIncome.Content = "Avg Income: " + econ.MeanIncome.FormatMoney();
            LblAvgWealth.Content = "Avg Wealth: " + econ.MeanNetWorth.FormatMoney();
            LblIncomeGini.Content = "Income Gini: " + econ.IncomeGini.ToString("0.00%");
            LblWealthGini.Content = "Wealth Gini: " + econ.WealthGini.ToString("0.00%");
            LblTotalCash.Content = "Total Cash: " + econ.TotalCash.FormatMoney();
            LblMoneySupply.Content = "Money Supply: " + econ.MoneySupply.FormatMoney();
            LblUnemployment.Content = "Unemployment: " + econ.Unemployment.ToString("0.00%");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }

}
