using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using System;

namespace EconSimVisual.Simulation.Helpers
{
    [Serializable]
    internal class JobListing : Entity
    {
        public Business Business { get; set; }
        public int Positions { get; set; }
        public double GrossPay { get; set; }
        public double NetPay => GrossPay * (1 - Taxes.Rates[TaxType.Income]);
    }
}
