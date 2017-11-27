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
        private readonly Func<LiveCharts.ChartPoint, string> labelPoint = chartPoint => chartPoint.Y.FormatMoney() + " (" + chartPoint.Participation.ToString("0.00%") + ")";

        private Government Government => SimulationScreen.Town.Agents.Government;
        private int chartsLastUpdated = 0;

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
            InitializeTaxPieChart();
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
            if (Entity.IsPeriodStart || Entity.Day - chartsLastUpdated > 30)
                UpdateTaxPieChart();

            GridIssuedBonds.SetData(Government.Debt.IssuedBonds);
        }

        private void UpdateLabels()
        {
            LblTaxesCurrent.Content = Government.Taxes.CurrentRevenue.FormatMoney();
            LblTaxesLast.Content = Government.Taxes.LastRevenue.FormatMoney();
            LblWelfareCurrent.Content = Government.Welfare.CurrentExpenses.FormatMoney();
            LblWelfareLast.Content = Government.Welfare.LastExpenses.FormatMoney();
            LblNetCurrent.UpdateColored(Government.Finances.CurrentSurplus);
            LblNetLast.UpdateColored(Government.Finances.LastSurplus);
            LblTotalDebt.Content = "Total Debt: " + Government.Debt.TotalAmount.FormatMoney();
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

            var currentBond = Government.Debt.CurrentBond;
            currentBond.FaceValue = faceValue;
            currentBond.UnitPrice = price;
            currentBond.MaturityDays = maturity;
            currentBond.Count = limit;
        }

        private static void TaxesChanged(object sender, RoutedEventArgs e)
        {
            if (!SimulationScreen.SimInitialized || (sender as DoubleUpDown)?.Value == null)
                return;
            var name = ((DoubleUpDown)sender).Name;
            var value = (double)((DoubleUpDown)sender).Value;
            var taxType = name.Substring(6, name.Length - 13).ToEnum<TaxType>();
            SimulationScreen.Town.Agents.Government.Taxes.Rates[taxType] = value;
        }

        private void InitializeTaxPieChart()
        {
            PieChartTaxes.Series = new SeriesCollection();
        }

        private void UpdateTaxPieChart()
        {
            chartsLastUpdated = Entity.Day;
            foreach (TaxType taxType in EnumUtils.GetValues<TaxType>())
            {
                var value = SimulationScreen.Town.Agents.Government.Taxes.LastRevenues[taxType];
                var series = PieChartTaxes.Series.Where(o => o.Title.Equals(taxType.ToString())).ToList();

                if (value == 0)
                {
                    if (series.Any()) PieChartTaxes.Series.Remove(series.First());
                }
                else
                {
                    if (series.Any())
                        series.First().Values = new ChartValues<double> { value };
                    else
                        PieChartTaxes.Series.Add(GetTaxPieSeries(taxType, value));
                }
            }
        }

        private PieSeries GetTaxPieSeries(TaxType taxType, double value)
        {
            return new PieSeries
            {
                Title = taxType.ToString(),
                DataLabels = true,
                Values = new ChartValues<double> { value },
                LabelPoint = labelPoint,
                FontSize = 18
            };
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
                label.Content = SimulationScreen.Town.Agents.Government.Taxes.LastRevenues[taxType]
                    .FormatMoney();
        }

        private void UpdateCurrentRevenue(TaxType taxType)
        {
            var label = (Label)FindName("Lbl" + taxType + "TaxCurrent");
            if (label != null)
                label.Content = SimulationScreen.Town.Agents.Government.Taxes.CurrentRevenues[taxType]
                    .FormatMoney();
        }
    }
}
