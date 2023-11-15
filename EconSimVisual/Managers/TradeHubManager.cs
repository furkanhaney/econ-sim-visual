using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EconSimVisual.Managers
{
    [Serializable]
    class TradeHubManager : Manager
    {
        public TradeHubManager(TradeHub tradeHub)
        {
            TradeHub = tradeHub;

        }

        protected TradeHub TradeHub { get; set; }
        protected double Margin { get; set; }

        public override void Manage()
        {
            Margin = Random.NextDouble(0.15, 0.35);
            ManageFunds();
            ManagePrices();
            ManageStocks();
        }

        private void ManageFunds()
        {
            if (TradeHub.Cash > 0)
                TradeHub.DepositCash(TradeHub.Cash);

            var current = TradeHub.Bonds.Current;
            if (TradeHub.Money < 25000)
                current.Count = current.OnSaleCount = 5;
            else
                current.Count = current.OnSaleCount = 0;
        }

        private void ManagePrices()
        {
            foreach (var good in AllGoods)
            {
                var marketPrice = (Town.Trade as TownTrade).GetUnitPrice(good);
                TradeHub.Prices[good] = marketPrice * (1 + Margin);
            }
        }

        private void ManageStocks()
        {
            TradeHub.TargetStocks[Good.Potato] = 25000;
            TradeHub.TargetStocks[Good.Squash] = 25000;
            TradeHub.TargetStocks[Good.Beer] = 500;
            TradeHub.TargetStocks[Good.Wine] = 0;
        }
    }
}
