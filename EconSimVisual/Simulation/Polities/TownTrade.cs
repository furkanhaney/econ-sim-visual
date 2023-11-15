using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Information;
using EconSimVisual.Simulation.Instruments.Securities;
using MoreLinq;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class TownTrade : PolityTrade
    {
        public TownTrade(Town town) : base(town)
        {
            DailyTransactions = new List<Transaction>();
        }

        private List<Transaction> DailyTransactions { get; }
        public bool UpdatePrices;
        public Dictionary<Good, double> LowestPrices = new Dictionary<Good, double>();
        private Town Town => Polity as Town;
        private Taxes Taxes
        {
            get
            {
                if (Town.Agents.Government == null)
                    return Town.SuperPolity.Agents.Government.Taxes;
                return Town.Agents.Government.Taxes;
            }
        }

        public double GetPrice(Agent buyer, Good good, double amount = 1)
        {
            var seller = GetSeller(buyer, good, amount);
            if (seller == null) return double.MaxValue;
            var price = seller.Prices[good];
            if (IsFinalGood(buyer, good)) price += Taxes.GetAmount(price, TaxType.Sales);
            return price;
        }

        public double GetUnitPrice(Good good)
        {
            if (UpdatePrices)
            {
                LowestPrices.Clear();
                var producers = Town.Agents.Manufacturers[good].ToList();
                var price = producers.Count > 0 ? producers.Average(o => o.Prices[good]) : 0;
                LowestPrices.Add(good, price);
                UpdatePrices = false;
                return price;
            }
            else if (!LowestPrices.Keys.Contains(good))
            {
                var producers = Town.Agents.Manufacturers[good].ToList();
                var price = producers.Count > 0 ? producers.Average(o => o.Prices[good]) : 0;
                LowestPrices.Add(good, price);
                return price;
            }
            else
            {
                return LowestPrices[good];
            }

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
            var seller = GetSeller(buyer, good, amount);
            if (seller == null)
                return;
            BuyGood(buyer, seller, good, amount);
        }

        public void BuyGood(Agent buyer, Vendor seller, Good good, double amount = 1)
        {
            var unitPrice = seller.Prices[good];
            var price = unitPrice * amount;
            var tax = IsFinalGood(buyer, good) ? Taxes.GetAmount(price, TaxType.Sales) : 0;

            if (price > 0)
                buyer.Pay(seller, price);
            if (tax > 0)
                Taxes.Pay(buyer, tax, TaxType.Sales);
            if (seller.Goods[good] < amount)
                MessageBox.Show("JEY");
            seller.Transfer(buyer, good, amount);
            seller.SalesCount[good] += amount;
            seller.Income.Sales += price;

            DailyTransactions.Add(new Transaction()
            {
                Goods = new GoodAmount(good, amount),
                UnitPrice = unitPrice,
                Buyer = buyer,
                Seller = seller,
                TaxPaid = tax
            });
        }

        public override void FirstTick()
        {
            base.FirstTick();
            DailyTransactions.Clear();
        }

        private static bool IsFinalGood(Agent buyer, Good good) => buyer is Person || good == Good.Capital;

        private Vendor GetSeller(Entity buyer, Good good, double amount = 1)
        {
            if (buyer is Person)
            {
                Vendor bestSeller = null;
                double bestPrice = double.MaxValue;
                foreach (var manufacturer in Town.Agents.Grocers)
                {
                    if (manufacturer.Goods[good] >= amount && manufacturer.Prices[good] < bestPrice)
                    {
                        bestSeller = manufacturer;
                        bestPrice = manufacturer.Prices[good];
                    }
                }
                return bestSeller;
            }
            else
            {
                Vendor bestSeller = null;
                double bestPrice = double.MaxValue;
                foreach (var manufacturer in Town.Agents.Manufacturers[good])
                {
                    if(manufacturer.Goods[good] >= amount && manufacturer.Prices[good] < bestPrice)
                    {
                        bestSeller = manufacturer;
                        bestPrice = manufacturer.Prices[good];
                    }
                }
                return bestSeller;
            }
        }

        protected override void UpdateTradeLogs()
        {
            var dailyLog = new TradeLog()
            {
                Polity = Town,
                FirstDay = Entity.Day
            };
            foreach (Good good in Enum.GetValues(typeof(Good)))
            {
                dailyLog.Summaries.Add(good, new GoodSummary()
                {
                    Good = good,
                    Price = GetUnitPrice(good),
                    Stocks = Town.Agents.Businesses.Sum(o => o.Goods[good]),
                    Production = Town.Agents.AllManufacturers.Where(o => o.MainGood == good).Sum(o => o.ActualOutput),
                    Volume = DailyTransactions.Where(o => o.Goods.Good == good).Sum(o => o.Goods.Amount)
                });
            }
            TradeLogs.Add(dailyLog);
        }
    }
}
