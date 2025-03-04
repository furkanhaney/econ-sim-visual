﻿using System.Collections.Generic;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Extensions;
    using Simulation.Base;
    using LiveCharts;
    using LiveCharts.Wpf;

    using Xceed.Wpf.Toolkit;

    /// <summary>
    /// Interaction logic for GovernmentPanel.xaml
    /// </summary>
    public partial class GovernmentPanel : IPanel
    {
        private Government Government => SimulationScreen.Polity.Agents.Government;

        public GovernmentPanel()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            CmbMaturity1.SetData(ChartsPanel.TimeFrames);
            foreach (var taxType in Enum.GetValues(typeof(TaxType)))
            {
                var updown = (DoubleUpDown)FindName("UpDown" + taxType + "TaxRate");
                updown.ValueChanged += TaxesChanged;
            }
        }

        public void Update()
        {
            UpdateWelfareExpenses(Government.Welfare);
            UpdateLabels();

            foreach (TaxType taxType in Enum.GetValues(typeof(TaxType)))
            {
                UpdateLastRevenue(taxType);
                UpdateCurrentRevenue(taxType);
            }
            if (Entity.IsPeriodStart || Entity.Day - PieChartTaxes.LastUpdated > 30)
                UpdateTaxPieChart();

            GridIssuedBonds.SetData(Government.Bonds.Issued);
        }

        private void UpdateLabels()
        {
            LblTaxesCurrent.Content = Government.Taxes.CurrentRevenue.FormatMoney();
            LblTaxesLast.Content = Government.Taxes.LastRevenue.FormatMoney();
            LblWelfareCurrent.Content = Government.Welfare.CurrentExpenses.FormatMoney();
            LblWelfareLast.Content = Government.Welfare.LastExpenses.FormatMoney();
            LblNetCurrent.UpdateColored(Government.Finances.CurrentSurplus);
            LblNetLast.UpdateColored(Government.Finances.LastSurplus);
            LblTotalDebt.Content = "Total Bonds: " + Government.Bonds.TotalAmount.FormatMoney();
        }

        public void AdjustPrograms(object sender, RoutedEventArgs e)
        {

            if (!SimulationScreen.SimInitialized || ((DoubleUpDown)sender).Value == null)
                return;
            var wellfare = Government.Welfare;
            wellfare.UniversalIncome.Wage = (double)UpDownUniversalIncome.Value;
            wellfare.UnemploymentWage.Wage = (double)UpDownUnemploymentWage.Value;
            wellfare.LowIncomeWage.Wage = (double)UpDownLowIncomeSupport.Value;
            wellfare.LowIncomeWage.Threshold = (double)UpDownLowIncomeSupportThreshold.Value;
        }

        public void AdjustBonds(object sender, RoutedEventArgs e)
        {
            if (!SimulationScreen.SimInitialized)
                return;
            if (UpDownFaceValue1.Value == null || UpDownPrice1.Value == null || UpDownBondLimit.Value == null)
                return;
            var faceValue = (double)UpDownFaceValue1.Value;
            var price = (double)UpDownPrice1.Value;
            var maturity = (int)CmbMaturity1.SelectedItem;
            var limit = (int)UpDownBondLimit.Value;
            LblYield1.Content = Finance.GetYield(faceValue, price, maturity).ToString("0.00%");

            var currentBond = Government.Bonds.Current;
            currentBond.FaceValue = faceValue;
            currentBond.UnitPrice = price;
            currentBond.MaturityDays = maturity;
            currentBond.Count = currentBond.OnSaleCount = limit;
        }

        private static void TaxesChanged(object sender, RoutedEventArgs e)
        {
            if (!SimulationScreen.SimInitialized || (sender as DoubleUpDown)?.Value == null)
                return;
            var name = ((DoubleUpDown)sender).Name;
            var value = (double)((DoubleUpDown)sender).Value;
            var taxType = name.Substring(6, name.Length - 13).ToEnum<TaxType>();
            SimulationScreen.Polity.Agents.Government.Taxes.Rates[taxType] = value;
        }

        private void UpdateTaxPieChart()
        {
            var list = new List<PieChartPoint>();
            foreach (var taxType in EnumUtils.GetValues<TaxType>())
            {
                var name = taxType.ToString().SplitCamelCase();
                var value = SimulationScreen.Polity.Agents.Government.Taxes.LastRevenues[taxType];
                list.Add(new PieChartPoint { Name = name, Value = value });
            }
            PieChartTaxes.Update(list);
        }

        private void UpdateWelfareExpenses(Welfare welfare)
        {
            LblCurrentExpensesUniversalIncome.Content = welfare.UniversalIncome.CurrentExpenses.FormatMoney();
            LblCurrentExpensesUnemploymentWage.Content = welfare.UnemploymentWage.CurrentExpenses.FormatMoney();
            LblCurrentExpensesLowIncomeSupport.Content = welfare.LowIncomeWage.CurrentExpenses.FormatMoney();

            LblLastExpensesUniversalIncome.Content = welfare.UniversalIncome.LastExpenses.FormatMoney();
            LblLastExpensesUnemploymentWage.Content = welfare.UnemploymentWage.LastExpenses.FormatMoney();
            LblLastExpensesLowIncomeSupport.Content = welfare.LowIncomeWage.LastExpenses.FormatMoney();
        }

        private void UpdateLastRevenue(TaxType taxType)
        {
            var label = (Label)FindName("Lbl" + taxType + "TaxLast");
            if (label != null)
                label.Content = SimulationScreen.Polity.Agents.Government.Taxes.LastRevenues[taxType]
                    .FormatMoney();
        }

        private void UpdateCurrentRevenue(TaxType taxType)
        {
            var label = (Label)FindName("Lbl" + taxType + "TaxCurrent");
            if (label != null)
                label.Content = SimulationScreen.Polity.Agents.Government.Taxes.CurrentRevenues[taxType]
                    .FormatMoney();
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var upDown = (DoubleUpDown)sender;
            if (upDown.Value is null)
                return;
            Government.LaborLaws.MinimumWage = (double)upDown.Value;
        }
    }
}
