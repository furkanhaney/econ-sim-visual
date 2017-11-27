using System.Collections.Generic;

namespace EconSimVisual.Simulation.Polities
{
    internal class ProperPolity : Polity
    {
        public List<Polity> SubPolities { get; } = new List<Polity>();
    }
}