using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Helpers
{
    internal class JobListing : Entity
    {
        public Business Business { get; set; }
        public double GrossPay { get; set; }
        public double NetPay => GrossPay * (1 - Taxes.Rates[TaxType.Income]);
    }
}
