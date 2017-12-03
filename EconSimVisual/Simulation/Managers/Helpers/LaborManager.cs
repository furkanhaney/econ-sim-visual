using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Helpers;
using MoreLinq;

namespace EconSimVisual.Simulation.Managers.Helpers
{
    internal class LaborManager : Manager
    {
        public LaborManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        private Manufacturer Manufacturer { get; }

        public override void Manage()
        {
            HandleJobOffers();
            HandleOverPaidWorkers();
        }

        private void HandleJobOffers()
        {
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
    }
}
