using System.Collections.Generic;
using System.Windows;

namespace EconSimVisual.Simulation.Managers
{
    using System;
    using System.Linq;
    using Extensions;
    using Agents;
    using Helpers;

    using MoreLinq;

    internal class ManufacturerManager : BusinessManager
    {
        public ManufacturerManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        private Manufacturer Manufacturer { get; }

        public override void Manage()
        {
            ManageStocks();
            ManagePrices();
            ManageCapital();
            ManageLabor();
            ManageFunds();
        }

        // TODO Add input costs
        private void ManageFunds()
        {
            Manufacturer.TargetCash = 10000 + 4 * Manufacturer.Labor.CurrentWages + 2 * Manufacturer.BankAccounts.Sum(o => o.MinimumPayment);
        }

        private void ManageStocks()
        {
            foreach (var input in Manufacturer.Process.Inputs)
                Manufacturer.TargetStocks[input.Good] = 10 + 1000 * Manufacturer.Process.ProductionConstant + 10 * input.Amount * Manufacturer.MaximalOutput;

            foreach (var output in Manufacturer.Process.Outputs)
                Manufacturer.TargetStocks[output.Good] = 10 + 1000 * Manufacturer.Process.ProductionConstant + 10 * output.Amount * Manufacturer.MaximalOutput;
        }

        // TODO Improve
        private void ManagePrices()
        {
            foreach (var good in Manufacturer.Prices.Keys.ToArray())
                if (Manufacturer.MaximalOutput == 0 || Manufacturer.ActualOutput != 0)
                {
                    var factor = Math.Log10(Manufacturer.TargetStocks[good]) - Math.Log10(Manufacturer.Goods[good] + 1);
                    Manufacturer.Prices[good] *= 1 + 0.017 * factor;
                }
        }

        // TODO Improve and refactor
        private void ManageCapital()
        {
            var cost = CalculateCostOfCapital();

            while (
                Manufacturer.MarginalRevenueCapital > cost &&
                Manufacturer.Money > 100 * K &&
                Manufacturer.Goods[Good.Capital] >= 1)
                Manufacturer.ShiftCapital(1);

            if (Manufacturer.Produces(Good.Capital))
                return;
            while (Manufacturer.MarginalRevenueCapital > cost &&
                   Manufacturer.Money > 100 * K &&
                   Town.Trade.CanBuyGood(Manufacturer, Good.Capital))
                Manufacturer.BuyCapital(1);
        }

        // TODO Improve
        private void ManageLabor()
        {
            HandleJobOffers();
            HandleOverPaidWorkers();
        }

        private void HandleJobOffers()
        {
            //var otherJobs = Town.JobsAvailable.Where(o => o.Business != Manufacturer).ToList();
            //var pay = Math.Min(Manufacturer.MarginalRevenueLabor, otherJobs.Count == 0 ? double.MaxValue : otherJobs.Median(o => o.GrossPay) * 1.5);
            if (Manufacturer.MarginalRevenueLabor < 0)
            {
                Town.JobsAvailable.RemoveAll(o => o.Business == Manufacturer);
                return;
            }

            var pay = Math.Max(1, Manufacturer.MarginalRevenueLabor);
            UpdateJobOffer(pay);
        }

        private void UpdateJobOffer(double pay)
        {
            if (Town.JobsAvailable.Count(o => o.Business == Manufacturer) == 0)
                Town.JobsAvailable.Add(new JobListing { Business = Manufacturer, GrossPay = pay });
            else
                Town.JobsAvailable.First(o => o.Business == Manufacturer).GrossPay = pay;
        }

        private void HandleOverPaidWorkers()
        {
            var overpaidWorkers = GetOverpaidWorkers();
            if (overpaidWorkers.Count > 0)
                Manufacturer.Labor.Fire(overpaidWorkers.MaxBy(o => o.Wage));
        }

        private ICollection<Worker> GetOverpaidWorkers()
        {
            var workers = Manufacturer.Labor.Workers.Where(w => w.Tenure >= 10);
            return workers.Where(o => Manufacturer.MarginalRevenueLabor * 1.1 < o.Wage).ToList();
        }

        private double CalculateCostOfCapital()
        {
            var cost = Manufacturer.CapitalDepreciationRate;
            if (Manufacturer.BankAccounts.Any())
                cost += Manufacturer.BankAccounts.Max(o => o.SavingsRate);
            return cost * Town.Trade.GetLastPrice(Good.Capital);
        }
    }
}