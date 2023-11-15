using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    class TradeLog
    {
        public Polity Polity { get; set; }
        public int FirstDay { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public Dictionary<Good, GoodSummary> Summaries { get; set; }

        public TradeLog()
        {
            Summaries = new Dictionary<Good, GoodSummary>();
        }

        /// <summary>
        /// Aggregates trade logs from multiple polities
        /// </summary>
        /// <returns></returns>
        public static TradeLog Aggregate(List<TradeLog> logs)
        {
            var newLog = new TradeLog();
            foreach (Good good in Enum.GetValues(typeof(Good)))
            {
                var meanPrice = logs.WeightedMean(
                    log => log.Summaries[good].Price,
                    log => log.Polity.Agents.Population.Count);
                newLog.Summaries.Add(good, new GoodSummary()
                {
                    Good = good,
                    Price = meanPrice,
                    Production = logs.Sum(log => log.Summaries[good].Production),
                    Stocks = logs.Sum(log => log.Summaries[good].Stocks),
                    Volume = logs.Sum(log => log.Summaries[good].Volume)
                });
            }
            return newLog;
        }
    }

    enum TimePeriod { Day, Week, Month, Quarter, Year }
}
