using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Information;

namespace EconSimVisual.Simulation.Polities
{
    internal class PolityEconomy
    {
        public PolityEconomy(Polity polity)
        {
            Polity = polity;
        }

        public List<EconomicReport> EconomicReports { get; set; } = new List<EconomicReport>();

        public double NominalGdp
        {
            get
            {
                var sum = Polity.Agents.Manufacturers.Sum(o =>
                    o.ActualOutput * ((Town)Polity).Trade.GetLastPrice(o.Process.Outputs[0].Good));
                return sum * 365;
            }
        }

        public double IncomeGini => Citizens.Select(o => o.NetIncome).Gini();
        public double WealthGini => Citizens.Select(o => o.NetWorth).Gini();
        public double TotalCash => Polity.Agents.All.Sum(o => o.Cash) - Polity.Agents.CentralBank.Cash;
        public double TotalDeposits => Polity.Agents.All.Sum(o => o.CheckingBalance);
        public double MoneySupply => Polity.Agents.All.Sum(o => o.Money) - Polity.Agents.CentralBank.Money;
        public double Unemployment => (double)Citizens.Count(o => !o.IsWorking) / Citizens.Count;
        public double AverageHunger => Polity.Agents.Population.Average(o => o.Hunger);
        public double MeanIncome => Citizens.Average(o => o.NetIncome);
        public double MeanNetWorth => Citizens.Average(o => o.NetWorth);
        public double MedianIncome => Citizens.Median(o => o.NetIncome);
        public double MedianNetWorth => Citizens.Median(o => o.NetIncome);
        public double TotalWealth => Citizens.Sum(o => o.NetWorth);

        private Polity Polity { get; }

        private List<Person> Citizens => Polity.Agents.Population;

        public void Tick()
        {
            UpdateEconomicsReports();
        }

        private void UpdateEconomicsReports()
        {
            EconomicReports.Add(new EconomicReport()
            {
                NominalGdp = NominalGdp,
                TotalWealth = TotalWealth,
                IncomeGini = IncomeGini,
                WealthGini = WealthGini,
                TotalCash = TotalCash,
                MoneySupply = MoneySupply,
                Unemployment = Unemployment,
                MeanIncome = MeanIncome,
                MedianIncome = MedianIncome
            });

        }

    }
}
