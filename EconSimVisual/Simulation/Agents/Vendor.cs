namespace EconSimVisual.Simulation.Agents
{
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;
    using Helpers;

    /// <summary>
    ///     Business that engages in the buying and selling of goods.
    /// </summary>
    internal abstract class Vendor : Business
    {
        protected Vendor(IEnumerable<Good> goodsSold)
        {
            TargetCash = 25000;
            TargetStocks = CollectionsExtensions.InitializeDictionary<Good>();
            Prices = CollectionsExtensions.InitializeDictionary(goodsSold, 10);
            SalesCount = CollectionsExtensions.InitializeDictionary(goodsSold);
            LastSalesCount = CollectionsExtensions.InitializeDictionary(goodsSold);
        }

        public Dictionary<Good, double> TargetStocks { get; }
        public Dictionary<Good, double> Prices { get; }
        public Dictionary<Good, double> SalesCount { get; }
        public Dictionary<Good, double> LastSalesCount { get; private set; }

        public override double Revenues => LastSalesRevenue;
        public override double Expenses => Labor.LastWagesPaid + LastStockExpenses;
        public double StockExpenses { get; protected set; }
        public double SalesRevenue { get; set; }

        public double LastSalesRevenue { get; private set; }
        public double LastStockExpenses { get; private set; }

        public override void LastTick()
        {
            AdjustStocks();
            base.LastTick();
        }

        protected override void ResetStats()
        {
            LastSalesRevenue = SalesRevenue;
            LastStockExpenses = StockExpenses;
            LastSalesCount = SalesCount.Clone();

            foreach (var good in SalesCount.Keys.ToArray())
                SalesCount[good] = 0;
            StockExpenses = 0;
            SalesRevenue = 0;

            base.ResetStats();
        }

        private void AdjustStocks()
        {
            var prevMoney = Cash;
            foreach (var good in TargetStocks.Keys)
            {
                if (this is Manufacturer manu && manu.Produces(good)) continue;
                while (Goods[good] < TargetStocks[good] && Town.Trade.CanBuyGood(this, good, 5))
                    Town.Trade.BuyGood(this, good, 5);
            }

            StockExpenses = prevMoney - Cash;
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