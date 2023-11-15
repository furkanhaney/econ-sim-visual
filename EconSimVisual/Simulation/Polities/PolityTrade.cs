using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Information;
using EconSimVisual.Simulation.Instruments.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    abstract class PolityTrade
    {
        public Polity Polity { get; }
        public virtual List<TradeLog> TradeLogs { get; }
        private SecurityExchange<Bond> bondExchange = null;
        private SecurityExchange<Stock> stockExchange = null;
        public SecurityExchange<Bond> BondExchange
        {
            get
            {
                if (bondExchange != null)
                    return bondExchange;
                return Polity.SuperPolity.Trade.BondExchange;
            }
            set
            {
                bondExchange = value;
            }
        }
        public SecurityExchange<Stock> StockExchange
        {
            get
            {
                if (stockExchange != null)
                    return stockExchange;
                return Polity.SuperPolity.Trade.StockExchange;
            }
            set
            {
                stockExchange = value;
            }
        }

        public PolityTrade(Polity polity)
        {
            Polity = polity;
            TradeLogs = new List<TradeLog>();
        }

        public virtual void FirstTick()
        {

        }

        public virtual void LastTick()
        {
            UpdateTradeLogs();
        }

        protected abstract void UpdateTradeLogs();
    }
}
