using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Information;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class PolityEconomy
    {
        public PolityEconomy(Polity polity)
        {
            Polity = polity;
            EconomicReports.Add(new EconomicReport());
        }

        public List<EconomicReport> EconomicReports { get; set; } = new List<EconomicReport>();

        public virtual double NominalGdp
        {
            get
            {
                if (Polity.Trade.TradeLogs.Count == 0)
                    return 0;
                var lastLog = Polity.Trade.TradeLogs.Last();
                var sum = Polity.Agents.AllManufacturers.Sum(o =>
                    o.ActualOutput * lastLog.Summaries[o.Process.Outputs[0].Good].Price);
                return sum * 365;
            }
        }

        public double IncomeGini => EconomicReports.Last().IncomeGini;
        public double WealthGini => EconomicReports.Last().WealthGini;
        public double TotalCash => EconomicReports.Last().TotalCash;
        public double MoneySupply => EconomicReports.Last().TotalCash;
        public double Unemployment => EconomicReports.Last().Unemployment;
        public double AverageHunger => Polity.Agents.Population.Average(o => o.Hunger);
        public double MaxHunger => Polity.Agents.Population.Max(o => o.Hunger);
        public double MeanIncome => EconomicReports.Last().Unemployment;
        public double MeanNetWorth => Citizens.Average(o => o.NetWorth);
        public double MedianIncome => EconomicReports.Last().MedianIncome;
        public double TotalWealth => Citizens.Sum(o => o.NetWorth);
        public double TotalReserves
        {
            get
            {
                return 0;
                return Polity.Agents.CentralBank.Deposits.Accounts.Where(o => o.Key is CommercialBank)
                    .Sum(o => o.Value.Balance);
            }
        }
        public double MonetaryBase => TotalCash + TotalReserves;
        public double MoneyMultiplier => MoneySupply / MonetaryBase;

        protected Polity Polity { get; set; }

        private List<Person> Citizens => Polity.Agents.Population;

        public void Tick()
        {
            if (Entity.Day % 30 == 0)
                UpdateEconomicsReports();
        }

        private void UpdateEconomicsReports()
        {
            int sampleSize = 100;
            var sample = Citizens.Sample(sampleSize);
            EconomicReports.Add(new EconomicReport()
            {
                NominalGdp = sample.Select(o => o.NetIncome).Gini(),
                TotalWealth = sample.Select(o => o.NetWorth).Sum(),
                IncomeGini = sample.Select(o => o.NetIncome).Gini(),
                WealthGini = sample.Select(o => o.NetWorth).Gini(),
                TotalCash = sample.Select(o => o.Cash).Sum(),
                MoneySupply = 0,
                Unemployment = (double)sample.Count(o => !o.IsWorking) / sampleSize,
                MeanIncome = sample.Average(o => o.NetIncome),
                MedianIncome = sample.Median(o => o.NetIncome)
            });
        }

    }
}
