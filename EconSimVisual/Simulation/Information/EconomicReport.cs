namespace EconSimVisual.Simulation.Information
{
    internal class EconomicReport
    {
        public double NominalGdp { get; set; }
        public double TotalWealth { get; set; }
        public double TotalCash { get; set; }
        public double MoneySupply { get; set; }
        public double IncomeGini { get; set; }
        public double WealthGini { get; set; }
        public double Unemployment { get; set; }
        public double MedianIncome { get; set; }
        public double MeanIncome { get; set; }
    }

    public enum MetricType { Mean, Median, Total }
}
