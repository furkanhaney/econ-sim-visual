using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Information;

namespace EconSimVisual.Simulation.Town
{
    internal class EconomyTracker
    {
        public EconomyTracker(Town town)
        {
            Town = town;
        }

        public List<EconomicReport> EconomicReports { get; set; } = new List<EconomicReport>();

        public double NominalGdp
        {
            get
            {
                return 0;
            }
        }

        public double IncomeGini => Citizens.Select(o => o.NetIncome).Gini();
        public double WealthGini => Citizens.Select(o => o.NetWorth).Gini();
        public double TotalCash => Town.Agents.All.Sum(o => o.Cash);
        public double MoneySupply => Town.Agents.All.Sum(o => o.Money);
        public double Unemployment => (double)Citizens.Count(o => !o.IsWorking) / Citizens.Count;
        public double AverageHunger => Town.Agents.Population.Average(o => o.Hunger);
        public double MeanIncome => Citizens.Average(o => o.NetIncome);
        public double MeanNetWorth => Citizens.Average(o => o.NetWorth);
        public double MedianIncome => Citizens.Median(o => o.NetIncome);
        public double MedianNetWorth => Citizens.Median(o => o.NetIncome);
        public double TotalWealth => Citizens.Sum(o => o.NetWorth);

        private Town Town { get; }

        private List<Person> Citizens => Town.Agents.Population;

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
