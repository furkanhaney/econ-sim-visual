using EconSimVisual.Managers.Helpers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Instruments.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Managers
{
    [Serializable]
    class FundManager : Manager
    {
        public FundManager(MutualFund fund)
        {
            Fund = fund;
        }

        public MutualFund Fund { get; }
        public Stock Stock { get; protected set; }

        public double UnitPrice
        {
            get
            {
                var totalValue = Fund.BalanceSheet.TotalEquity;
                return totalValue / Fund.Owners.OutstandingShares;
            }
        }

        public override void Manage()
        {
            ManageInvestments();
            ManageOwnStock();
        }

        private void ManageOwnStock()
        {
            if (Stock == null)
                InitializeStock();
            else
            {
                var exchange = Fund.Town.Trade.StockExchange;
                if (!exchange.All.Contains(Stock))
                    exchange.All.Add(Stock);
                Stock.UnitPrice = UnitPrice;
                Stock.Count = Stock.OnSaleCount = 10000;
                Stock.IsIssued = false;
            }
        }

        private void ManageInvestments()
        {
            if (Fund.Cash > 0)
                Fund.DepositCash(Fund.Cash);
            if (Fund.Type == FundType.Equity)
                new SecurityManager(Fund).BuyStocks(Fund.Money);
            else if (Fund.Type == FundType.Bond)
                new SecurityManager(Fund).BuyBonds(Fund.Money);
        }

        private void InitializeStock()
        {
            Stock = new Stock()
            {
                IsIssued = false,
                Issuer = Fund,
                Count = 10000,
                UnitPrice = UnitPrice
            };
            var exchange = Fund.Town.Trade.StockExchange;
            exchange.All.Add(Stock);
        }
    }
}
