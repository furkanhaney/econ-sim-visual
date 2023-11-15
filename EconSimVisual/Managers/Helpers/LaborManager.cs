using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Ink;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using MoreLinq;

namespace EconSimVisual.Managers.Helpers
{
    [Serializable]
    internal class LaborManager : Manager
    {
        private LaborLaws Laws => Manufacturer.Government.LaborLaws;

        public LaborManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        private Manufacturer Manufacturer { get; }

        public override void Manage()
        {
            HandleJobOffers();
            HandleOverPaidWorkers();
            IncraseLowWages();
        }

        private void HandleJobOffers()
        {
            if (Manufacturer.MarginalRevenueLabor < 0)
            {
                Town.JobsAvailable.RemoveAll(o => o.Business == Manufacturer);
                return;
            }

            var pay = Math.Max(Laws.MinimumWage, Manufacturer.MarginalRevenueLabor * 0.8);
            UpdateJobOffer(pay);
        }

        private void IncraseLowWages()
        {
            foreach (var worker in Manufacturer.Labor.Workers)
                if (worker.Wage < Laws.MinimumWage)
                    worker.Wage = Laws.MinimumWage;
        }

        private void UpdateJobOffer(double pay)
        {
            if (Town.JobsAvailable.Count(o => o.Business == Manufacturer) == 0)
                Town.JobsAvailable.Add(new JobListing { Town = Town, Business = Manufacturer, GrossPay = pay });
            else
            {
                var listing = Town.JobsAvailable.First(o => o.Business == Manufacturer);
                listing.GrossPay = pay;
                listing.Positions = 5;
            }
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
    }
}
