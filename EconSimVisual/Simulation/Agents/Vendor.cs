namespace EconSimVisual.Simulation.Agents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using EconSimVisual.Simulation.Polities;
    using Extensions;
    using Helpers;

    [Serializable]
    /// <summary>
    ///     Business that engages in the buying and selling of goods.
    /// </summary>
    internal abstract class Vendor : Business
    {
        protected Vendor(IEnumerable<Good> goodsSold)
        {
            TargetStocks = CollectionsExtensions.InitializeDictionary<Good>();
            Prices = CollectionsExtensions.InitializeDictionary(goodsSold, 10);
            SalesCount = CollectionsExtensions.InitializeDictionary(goodsSold);
            LastSalesCount = CollectionsExtensions.InitializeDictionary(goodsSold);
        }

        public Dictionary<Good, double> TargetStocks { get; }
        public Dictionary<Good, double> Prices { get; }
        public Dictionary<Good, double> SalesCount { get; }
        public Dictionary<Good, double> LastSalesCount { get; private set; }

        public override void LastTick()
        {
            if (Rnd.NextDouble() < 0.25)
                AdjustStocks();
            base.LastTick();
        }

        protected override void ResetStats()
        {
            LastSalesCount = SalesCount.Clone();

            foreach (var good in SalesCount.Keys.ToArray())
                SalesCount[good] = 0;

            base.ResetStats();
        }

        protected virtual void AdjustStocks()
        {
            var prevMoney = Cash;
            var trade = Town.Trade as TownTrade;
            foreach (var good in TargetStocks.Keys.ToList().Shuffle())
            {
                if (this is Manufacturer manu && manu.MainGood == good) continue;
                var amount = TargetStocks[good] - Goods[good];
                if (Goods[good] * 2 > TargetStocks[good])
                    continue;
                while (!trade.CanBuyGood(this, good, amount) && amount >= 200)
                    amount /= 2;
                if (trade.CanBuyGood(this, good, amount))
                    trade.BuyGood(this, good, amount);
            }
            Income.Inventory = prevMoney - Cash;
        }

        public string GoodsUI
        {
            get { return string.Join(",", Prices.Keys.Select(o => o.ToString()).ToArray()); }
        }

        public string PricesUI
        {
            get
            {
                var prices = new List<string>();
                foreach (var good in Prices)
                    prices.Add(good.Value.FormatMoney());
                return string.Join(",", prices);
            }
        }

        public string SalesUI
        {
            get
            {
                var sales = new List<string>();
                foreach (var good in LastSalesCount)
                    sales.Add(good.Value.FormatAmount());
                return string.Join(",", sales);
            }
        }

        public string StocksUI
        {
            get
            {
                var stocks = new List<string>();
                foreach (var good in SalesCount)
                    stocks.Add(Goods[good.Key].FormatAmount());
                return string.Join(",", stocks);
            }
        }

    }
}