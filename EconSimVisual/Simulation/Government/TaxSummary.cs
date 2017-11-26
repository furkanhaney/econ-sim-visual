using System.Collections.Generic;

namespace EconSimVisual.Simulation.Government
{
    internal class TaxSummary
    {
        public int Day { get; set; }
        public Dictionary<TaxType, double> Rates { get; set; }
        public Dictionary<TaxType, double> Revenues { get; set; }
    }
}
