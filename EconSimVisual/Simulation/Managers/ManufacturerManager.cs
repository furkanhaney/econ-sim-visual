using System.Collections.Generic;
using System.Windows;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Managers
{
    using System;
    using System.Linq;
    using Extensions;
    using Agents;
    using Helpers;

    using MoreLinq;

    internal class ManufacturerManager : Manager
    {
        public ManufacturerManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
            CapitalManager = new CapitalManager(Manufacturer);
            LaborManager = new LaborManager(Manufacturer);
        }

        private Manufacturer Manufacturer { get; }
        private CapitalManager CapitalManager { get; }
        private LaborManager LaborManager { get; }

        public override void Manage()
        {
            CapitalManager.Manage();
            LaborManager.Manage();

            ManageFunds();
            ManageStocks();
            ManagePrices();
            ManageDividends();
        }

        private void ManageFunds()
        {
            if (Manufacturer.Cash > 0)
                Manufacturer.DepositCash(Manufacturer.Cash);
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
                    Manufacturer.Prices[good] *= 1 + 0.03 * factor;
                }
        }

        private void ManageDividends()
        {
            double dividendAmount;
            if (Manufacturer.Money > 200000)
                dividendAmount = (Manufacturer.Money - 200000) / Manufacturer.Owners.OutstandingShares;
            else
                dividendAmount = 0;

            Manufacturer.Owners.Dividends = dividendAmount;
        }
    }
}