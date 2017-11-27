using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    internal class TownAgents : PolityAgents
    {
        public override List<Person> Population { get; } = new List<Person>();
        public override List<Manufacturer> Manufacturers { get; } = new List<Manufacturer>();
        public override List<Grocer> Grocers { get; } = new List<Grocer>();
        public override List<CommercialBank> Banks { get; } = new List<CommercialBank>();
    }
}