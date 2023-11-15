using System;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Managers.Helpers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Managers
{
    [Serializable]
    internal class ManufacturerManager : Manager
    {
        public ManufacturerManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
            CapitalManager = new CapitalManager(Manufacturer);
            LaborManager = new LaborManager(Manufacturer);
        }

        private static Random rnd = new Random();
        private double PriceAdjustmentSpeed = rnd.NextDouble() * 0.02 + 0.01;
        private Manufacturer Manufacturer { get; }
        private CapitalManager CapitalManager { get; }
        private LaborManager LaborManager { get; }
        public override Town Town
        {
            get => base.Town; set
            {
                base.Town = value;
                CapitalManager.Town = value;
                LaborManager.Town = value;
            }
        }

        public override void Manage()
        {
            if (rnd.NextDouble() < 0.2)
            {
                CapitalManager.Manage();
                LaborManager.Manage();
                ManageBonds();
                ManageFunds();
                ManageStocks();
                ManagePrices();
                ManageDividends();
                ManageBanktrupcy();
            }
        }

        private void ManageBanktrupcy()
        {
            var equity = Manufacturer.BalanceSheet.TotalEquity;
            var assets = Manufacturer.BalanceSheet.TotalAssets;
            if (equity < 0 && assets + equity < 0)
            {
                Manufacturer.Town.Agents.AllManufacturers.Remove(Manufacturer);
                foreach (var stock in Manufacturer.Owners.IssuedStocks)
                    stock.Owner = null;
                Manufacturer.Pay(Government, Manufacturer.Money);
                foreach (var account in Manufacturer.BankAccounts)
                    account.Bank.Deposits.Accounts.Remove(Manufacturer);
                foreach (var bond in Manufacturer.Bonds.Issued)
                    bond.Owner = null;
                Manufacturer.Town.Trade.BondExchange.All.RemoveAll(o => o.Issuer == Manufacturer);
                Manufacturer.Town.Trade.StockExchange.All.RemoveAll(o => o.Issuer == Manufacturer);
                foreach (var worker in Manufacturer.Labor.Workers.ToList())
                    Manufacturer.Labor.Fire(worker);
            }
        }

        private void ManageFunds()
        {
            if (Manufacturer.Cash > 0)
                Manufacturer.DepositCash(Manufacturer.Cash);
            if (Manufacturer.Money < 200000)
            {
                var current = Manufacturer.Bonds.Current;
                current.Count = current.OnSaleCount = 50;
            }
        }

        private void ManageBonds()
        {
            var capitalReturn = CapitalManager.CapitalReturn;
            var current = Manufacturer.Bonds.Current;
            if (capitalReturn < 0 || Manufacturer.Money > 250000)
                StopBondSales();
            else
            {
                var yield = Math.Min(0.1, capitalReturn);
                current.Count = current.OnSaleCount = 5;
                current.FaceValue = 1000;
                current.MaturityDays = 365;
                current.UnitPrice = Finance.GetPrice(1000, yield, 365);
            }
        }

        private void StopBondSales()
        {
            var current = Manufacturer.Bonds.Current;
            current.Count = current.OnSaleCount = 0;
        }

        private void ManageStocks()
        {
            foreach (var input in Manufacturer.Process.Inputs)
                Manufacturer.TargetStocks[input.Good] = 10 + 1000 * Manufacturer.Process.ProductionConstant + 10 * input.Amount * Manufacturer.MaximalOutput;

            foreach (var output in Manufacturer.Process.Outputs)
                Manufacturer.TargetStocks[output.Good] = 10 + 1000 * Manufacturer.Process.ProductionConstant + 10 * output.Amount * Manufacturer.MaximalOutput;
        }

        private void ManagePrices()
        {
            foreach (var good in Manufacturer.Prices.Keys.ToArray())
                if (Manufacturer.MaximalOutput == 0 || Manufacturer.ActualOutput != 0)
                {
                    var factor = Math.Log10(Manufacturer.TargetStocks[good] + 100) - Math.Log10(Manufacturer.Goods[good] + 100);
                    Manufacturer.Prices[good] *= 1 + PriceAdjustmentSpeed * factor;
                    (Town.Trade as TownTrade).UpdatePrices = true;
                }
        }

        private void ManageDividends()
        {
            double dividendAmount;
            if (Manufacturer.Money > 1000000)
                dividendAmount = (Manufacturer.Money - 1000000) / Manufacturer.Owners.OutstandingShares;
            else
                dividendAmount = 0;

            Manufacturer.Owners.Dividends = dividendAmount;
        }
    }
}