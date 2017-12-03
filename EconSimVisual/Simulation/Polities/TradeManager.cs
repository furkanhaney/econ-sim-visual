using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Information;
using EconSimVisual.Simulation.Instruments.Securities;
using MoreLinq;

namespace EconSimVisual.Simulation.Polities
{
    internal class TradeManager
    {
        public TradeManager(Town town)
        {
            Town = town;
            TradeLogs = new List<List<GoodSummary>>();
            BondExchange = new SecurityExchange<Bond>();
            StockExchange = new SecurityExchange<Stock>();
            DailyTransactions = new List<Transaction>();
        }

        public SecurityExchange<Bond> BondExchange { get; }
        public SecurityExchange<Stock> StockExchange { get; }
        public List<List<GoodSummary>> TradeLogs { get; }

        private List<Transaction> DailyTransactions { get; }
        private Town Town { get; }
        private Taxes Taxes => Town.Agents.Government.Taxes;

        public double GetPrice(Agent buyer, Good good, double amount = 1)
        {
            var seller = GetSeller(buyer, good, amount);
            if (seller == null) return double.MaxValue;
            var price = seller.Prices[good];
            if (IsFinalGood(buyer, good)) price += Taxes.GetAmount(price, TaxType.Sales);
            return price;
        }

        public double GetLastPrice(Good good)
        {
            var producers = Town.Agents.Manufacturers.Where(m => m.Produces(good)).ToList();
            return producers.Count > 0 ? producers.Min(o => o.Prices[good]) : 0;
        }

        public bool CanBuyGood(Agent buyer, Good good, double amount = 1)
        {
            var seller = GetSeller(buyer, good, amount);
            if (seller == null)
                return false; // No seller available

            var price = amount * seller.Prices[good];
            if (IsFinalGood(buyer, good))
                price += Taxes.GetAmount(price, TaxType.Sales);

            return price == 0 || buyer.CanPay(price); // Can afford the price
        }

        public void BuyGood(Agent buyer, Good good, double amount = 1)
        {
            Debug.Assert(CanBuyGood(buyer, good, amount));

            var seller = GetSeller(buyer, good, amount);
            var unitPrice = seller.Prices[good];
            var price = unitPrice * amount;
            var tax = IsFinalGood(buyer, good) ? Taxes.GetAmount(price, TaxType.Sales) : 0;

            if (price > 0)
                buyer.Pay(seller, price);
            if (tax > 0)
                Taxes.Pay(buyer, tax, TaxType.Sales);

            seller.Transfer(buyer, good, amount);
            seller.SalesCount[good] += amount;
            seller.SalesRevenue += price;

            // Makes sure that all grocers get share.
            if (seller is Grocer)
                seller.Prices[good] *= 1.001;

            DailyTransactions.Add(new Transaction()
            {
                Goods = new GoodAmount(good, amount),
                UnitPrice = unitPrice,
                Buyer = buyer,
                Seller = seller,
                TaxPaid = tax
            });
        }

        public void FirstTick()
        {
            DailyTransactions.Clear();
        }

        public void LastTick()
        {
            UpdateTradeLogs();
        }

        private static bool IsFinalGood(Agent buyer, Good good) => buyer is Person || good == Good.Capital;

        private Vendor GetSeller(Entity buyer, Good good, double amount = 1)
        {
            if (buyer is Person)
            {
                var grocers = Town.Agents.Grocers.Where(g => g.Goods[good] >= amount).ToList();
                if (grocers.Count > 0)
                    return grocers.MinBy(g => g.Prices[good]);
            }
            else if (buyer is Vendor)
            {
                var manufacturers = Town.Agents.Manufacturers.Where(m => m.Produces(good) && m.Goods[good] >= amount && m != buyer).ToList();
                if (manufacturers.Count > 0)
                    return manufacturers.MinBy(m => m.Prices[good]);
            }
            return null;
        }

        private void UpdateTradeLogs()
        {
            var logs = new List<GoodSummary>();
            foreach (Good good in Enum.GetValues(typeof(Good)))
            {
                var newPrice = GetLastPrice(good);
                var oldPrice = TradeLogs.Count == 0 ? newPrice : TradeLogs.Last().First(o => o.Good == good).Price;
                logs.Add(new GoodSummary()
                {
                    Good = good,
                    Price = newPrice,
                    PriceChange = newPrice - oldPrice,
                    Stocks = Town.Agents.Businesses.Sum(o => o.Goods[good]),
                    Production = Town.Agents.Manufacturers.Where(o => o.Produces(good)).Sum(o => o.ActualOutput),
                    Volume = DailyTransactions.Where(o => o.Goods.Good == good).Sum(o => o.Goods.Amount)
                });
            }

            TradeLogs.Add(logs);
        }
    }
}
