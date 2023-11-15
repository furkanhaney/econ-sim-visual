using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    class ProperPolityTrade : PolityTrade
    {
        public ProperPolityTrade(ProperPolity properPolity) : base(properPolity)
        {

        }

        public ProperPolity ProperPolity => Polity as ProperPolity;

        protected override void UpdateTradeLogs()
        {
            var newLog = TradeLog.Aggregate(ProperPolity.SubPolities.Select(o => o.Trade.TradeLogs.Last()).ToList());
            TradeLogs.Add(newLog);
        }
    }
}
