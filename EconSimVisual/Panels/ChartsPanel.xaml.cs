using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Information;
using EconSimVisual.Simulation.Town;
using LiveCharts;
using LiveCharts.Wpf;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for ChartsPanel.xaml
    /// </summary>
    public partial class ChartsPanel : IPanel
    {
        public static IEnumerable<int> TimeFrames => new[] { 30, 90, 360 };
        public IEnumerable<PropertyInfo> GetProperties<T>() => typeof(T).GetProperties().Where(o => o.PropertyType == typeof(double));
        public List<ChartPoint> GoodsChartPoints { get; set; } = new List<ChartPoint>();
        public List<ChartPoint> EconomicsChartPoints { get; set; } = new List<ChartPoint>();

        public bool UiInitialized;

        public ChartsPanel()
        {
            InitializeComponent();
            DataContext = this;
            InitializeUi();
            UiInitialized = true;
        }

        public static ChartPoint GetChartPoint(Good good, PropertyInfo prop)
        {
            return new ChartPoint(GetGoodValue(good, prop), "Day " + Entity.Day);
        }
        public static ChartPoint GetChartPoint(PropertyInfo prop)
        {
            return new ChartPoint(GetEconomicValue(prop), "Day " + Entity.Day);
        }

        private static ChartValues<double> GetGoodValues(Good good, PropertyInfo prop, int days = 30)
        {
            return new ChartValues<double>(Town.Current.Trade.TradeLogs.Select(o => (double)prop.GetValue(o.First(k => k.Good == good), null)).Reverse().Take(days).Reverse());
        }
        private static ChartValues<double> GetEconomicValues(PropertyInfo prop, int days = 30)
        {
            return new ChartValues<double>(Town.Current.Economy.EconomicReports.Select(o => (double)prop.GetValue(o, null)).Reverse().Take(days).Reverse());
        }
        private static List<string> CreateChartLabels(int maxCount)
        {
            var list = new List<string>();
            var day = Entity.Day;
            while (day > 0 && list.Count < maxCount)
                list.Add("Day " + day--);
            list.Reverse();
            return list;
        }
        private static Func<double, string> GetLabelFormatter(PropertyInfo prop)
        {
            var dollarProperties = new[] { "UnitPrice", "TotalCash", "PriceChange", "NominalGdp", "TotalWealth", "MoneySupply", "MedianIncome", "MeanIncome" };
            var amountProperties = new[] { "Stocks", "Production", "Volume" };
            var percentageProperties = new[] { "IncomeGini", "WealthGini", "Unemployment" };

            if (dollarProperties.Contains(prop.Name))
                return value => value.FormatMoney();
            if (amountProperties.Contains(prop.Name))
                return value => value.FormatAmount();
            if (percentageProperties.Contains(prop.Name))
                return value => value.ToString("0.00%");
            throw new ArgumentOutOfRangeException();
        }
        private static LineSeries CreateLineSeries(string title, IChartValues values)
        {
            return new LineSeries
            {
                Title = title,
                LineSmoothness = 0,
                Values = values
            };
        }
        private static double GetGoodValue(Good good, PropertyInfo prop)
        {
            return (double)prop.GetValue(Town.Current.Trade.TradeLogs.Last().First(o => o.Good == good), null);
        }
        private static double GetEconomicValue(PropertyInfo prop)
        {
            return (double)prop.GetValue(Town.Current.Economy.EconomicReports.Last(), null);
        }

        public void ReadGoodInputs(out Good good, out PropertyInfo prop, out int timeFrame)
        {
            good = (Good)CmbGood.SelectedItem;
            prop = GetProperties<GoodSummary>().First(o => o.Name.SplitCamelCase().Equals(CmbGoodMetric.SelectedItem));
            timeFrame = (int)CmbGoodTimeFrame.SelectedItem;
        }
        public void ReadEconomicInputs(out PropertyInfo prop, out int timeFrame)
        {
            prop = GetProperties<EconomicReport>()
                .First(o => o.Name.SplitCamelCase().Equals(CmbIndicator.SelectedItem));
            timeFrame = (int)CmbEconomicTimeFrame.SelectedItem;
        }
        public void UpdateCharts(IList<ChartPoint> chartPoints)
        {
            ReadGoodInputs(out var good, out var prop, out var timeFrame);
            for (var i = 0; i < chartPoints.Count; i++)
            {
                ChartGoods.Series[0].Values.Add(chartPoints[i].AxisX);
                AxisGoodsX.Labels.Add(chartPoints[i].AxisY);
            }
            chartPoints.Clear();
            while (ChartGoods.Series[0].Values.Count > timeFrame)
            {
                ChartGoods.Series[0].Values.RemoveAt(0);
                AxisGoodsX.Labels.RemoveAt((0));
            }
        }
        public void UpdateCharts2(IList<ChartPoint> chartPoints)
        {
            ReadEconomicInputs(out var prop, out var timeFrame);
            for (var i = 0; i < chartPoints.Count; i++)
            {
                ChartEconomy.Series[0].Values.Add(chartPoints[i].AxisX);
                AxisEconomyX.Labels.Add(chartPoints[i].AxisY);
            }
            chartPoints.Clear();
            while (ChartEconomy.Series[0].Values.Count > timeFrame)
            {
                ChartEconomy.Series[0].Values.RemoveAt(0);
                AxisEconomyX.Labels.RemoveAt((0));
            }
        }

        private void CmbGood_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UiInitialized)
                return;
            ResetGoodsChart();
        }
        private void InitializeUi()
        {
            CmbGoodMetric.ItemsSource = GetProperties<GoodSummary>().Select(o => o.Name.SplitCamelCase());
            CmbGood.ItemsSource = Enum.GetValues(typeof(Good));
            CmbGoodTimeFrame.ItemsSource = CmbEconomicTimeFrame.ItemsSource = TimeFrames;
            CmbIndicator.ItemsSource = GetProperties<EconomicReport>().Select(
                o => o.Name.SplitCamelCase());

            ChartGoods.Series = new SeriesCollection
            {
                CreateLineSeries("UnitPrice", new ChartValues<double>())
            };
            AxisGoodsY.Title = "UnitPrice";
            AxisGoodsY.LabelFormatter = value => value.FormatMoney();
            AxisGoodsX.Labels = new List<string>();

            ChartEconomy.Series = new SeriesCollection
            {
                CreateLineSeries("Nominal GDP", new ChartValues<double>())
            };
            AxisEconomyY.Title = "Nominal GDP";
            AxisEconomyY.LabelFormatter = value => value.FormatMoney();
            AxisEconomyX.Labels = new List<string>();
        }
        private void ResetGoodsChart()
        {
            ReadGoodInputs(out var good, out var prop, out var timeFrame);
            ChartGoods.Series[0] = CreateLineSeries(prop.Name.SplitCamelCase() + ":", GetGoodValues(good, prop, timeFrame));
            AxisGoodsX.Labels = CreateChartLabels(timeFrame);
            AxisGoodsY.Title = prop.Name.SplitCamelCase();
            AxisGoodsY.LabelFormatter = GetLabelFormatter(prop);
        }
        private void ResetEconomicsChart()
        {
            ReadEconomicInputs(out var prop, out var timeFrame);
            ChartEconomy.Series[0] = CreateLineSeries(prop.Name.SplitCamelCase() + ":", GetEconomicValues(prop, timeFrame));
            AxisEconomyX.Labels = CreateChartLabels(timeFrame);
            AxisEconomyY.Title = prop.Name.SplitCamelCase();
            AxisEconomyY.LabelFormatter = GetLabelFormatter(prop);
        }
        private void CmbIndicator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UiInitialized)
                return;
            ResetEconomicsChart();
        }
        private void CmbEconomicTimeFrame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UiInitialized)
                return;
            ResetEconomicsChart();
        }

        public void Update()
        {
            UpdateCharts(GoodsChartPoints);
            UpdateCharts2(EconomicsChartPoints);
        }

        public void Initialize()
        {

        }
    }

    public class ChartSettings
    {

    }

    public class ChartPoint
    {
        public double AxisX { get; }
        public string AxisY { get; }

        public ChartPoint(double axisX, string axisY)
        {
            AxisX = axisX;
            AxisY = axisY;
        }

    }
}
