using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using EconSimVisual.Initializers;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using Debug = EconSimVisual.Panels.Debug;

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
        public static World World { get; private set; }
        public static Polity Polity { get; private set; }
        public SimRunner Runner { get; }

        public SimulationScreen(World world)
        {
            File.WriteAllText(@"D:\logs.txt", "");
            World = world;
            Polity = World.TopPolities[0];
            InitializeGui();
            Runner = new SimRunner(World, this);
            SimInitialized = true;
            Progress();
            var testThread = new Thread(RunTests) { IsBackground = true };
            testThread.Start();
        }

        private void RunTests()
        {
            BankAccount.Test();
        }

        public static bool SimInitialized { get; set; }
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
            Debug.StartTime = DateTime.UtcNow;
            var days = CmbSimDays.Text.ParseDays();
            Runner.Run(days);
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
            CmbPolity.ItemsSource = World.AllPolities;
            CmbPolity.SelectedIndex = 0;
            DataContext = this;
        }

        public void Update()
        {
            ChartsPanel.ReadGoodInputs(out var good, out var prop1, out var timeFrame);
            ChartsPanel.ReadEconomicInputs(out var prop2, out var timeFrame2);
            ChartsPanel.GoodsChartPoints.Add(Panels.ChartsPanel.GetChartPoint(good, prop1));
            ChartsPanel.EconomicsChartPoints.Add(Panels.ChartsPanel.GetChartPoint(prop2));
            foreach (var panel in AllPanels.ToList())
                if (((panel as Control).Parent as TabItem).Visibility == Visibility.Visible)
                    panel.Update();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            var econ = Polity.Economy;

            LblDay.Content = "Date: " + FormatUtils.ToDate(Entity.Day).Format();
            LblPopulation.Content = "Population: " + Polity.Agents.Population.Count.ToString("###,##0");
            LblGdp.Content = "GDP: " + econ.NominalGdp.FormatMoney();
            //LblJobs.Content = "Available Jobs: " + Town.JobsAvailable.Count;
            LblAvgHunger.Content = "Max Hunger: " + Polity.Agents.Population.Max(o => o.Hunger).ToString("0");
            LblAvgIncome.Content = "Avg Income: " + econ.MeanIncome.FormatMoney();
            LblAvgWealth.Content = "Avg Wealth: " + econ.MeanNetWorth.FormatMoney();
            LblIncomeGini.Content = "Income Gini: " + econ.IncomeGini.ToString("0.00%");
            LblWealthGini.Content = "Wealth Gini: " + econ.WealthGini.ToString("0.00%");
            LblTotalCash.Content = "Total Cash: " + econ.TotalCash.FormatMoney();
            LblMoneySupply.Content = "Money Supply: " + econ.MoneySupply.FormatMoney();
            LblUnemployment.Content = "Unemployment: " + econ.Unemployment.ToString("0.00%");
            LblHappiness.Content = "Happiness: " + Polity.Agents.Population.Average(o => o.Happiness).ToString("0.00");

            if (Polity.Agents.Government != null)
            {
                LblTreasury.Content = "Treasury: " + Polity.Agents.Government.Money.FormatMoney();
                var debt = Polity.Agents.Government.BalanceSheet.TotalLiabilities;
                var percent = debt / Polity.Economy.NominalGdp;
                LblDebt.Content = "Debt: " + debt.FormatMoney();
                if (!double.IsNaN(percent))
                    LblDebt.Content += " (" + percent.ToString("%0.00") + ")";
            }
            else
            {
                LblTreasury.Content = "Treasury: -";
                LblDebt.Content = "Debt: -";
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CmbPolity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Polity = (Polity)CmbPolity.SelectedItem;
            if (Polity.Agents.Government == null)
                tabGovernment.Visibility = Visibility.Collapsed;
            else
                tabGovernment.Visibility = Visibility.Visible;
            if (Polity.Agents.CentralBank == null)
                tabCentralBank.Visibility = Visibility.Collapsed;
            else
                tabCentralBank.Visibility = Visibility.Visible;
            Update();
            ChartsPanel.ResetEconomicsChart();
            ChartsPanel.ResetGoodsChart();
        }
    }

}
