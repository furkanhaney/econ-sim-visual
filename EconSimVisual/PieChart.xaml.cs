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
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using LiveCharts;
using LiveCharts.Wpf;

namespace EconSimVisual
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart
    {
        private readonly Func<LiveCharts.ChartPoint, string> labelPoint = chartPoint => chartPoint.Y.FormatMoney() + " (" + chartPoint.Participation.ToString("0.00%") + ")";
        public int LastUpdated { get; set; }

        public PieChart()
        {
            InitializeComponent();
            chart.Series = new SeriesCollection();
        }

        public void Update(IEnumerable<PieChartPoint> points)
        {
            LastUpdated = Entity.Day;
            foreach (var point in points)
            {
                var series = chart.Series.Where(o => o.Title.Equals(point.Name)).ToList();

                if (point.Value == 0)
                {
                    if (series.Any()) chart.Series.Remove(series.First());
                }
                else
                {
                    if (series.Any())
                        series.First().Values = new ChartValues<double> { point.Value };
                    else
                        chart.Series.Add(GetTaxPieSeries(point));
                }
            }
        }

        private PieSeries GetTaxPieSeries(PieChartPoint point)
        {
            return new PieSeries
            {
                Title = point.Name,
                DataLabels = true,
                Values = new ChartValues<double> { point.Value },
                LabelPoint = labelPoint,
                FontSize = 18
            };
        }

    }

    public class PieChartPoint
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
