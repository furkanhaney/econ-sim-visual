using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    internal class ProperPolityAgents : PolityAgents
    {
        public ProperPolity Polity { get; }
        public ProperPolityAgents(ProperPolity polity)
        {
            Polity = polity;
        }

        public override List<Person> Population => Polity.SubPolities.SelectMany(o => o.Agents.Population).ToList();
        public override List<Manufacturer> Manufacturers => Polity.SubPolities.SelectMany(o => o.Agents.Manufacturers).ToList();
        public override List<Grocer> Grocers => Polity.SubPolities.SelectMany(o => o.Agents.Grocers).ToList();
        public override List<CommercialBank> Banks => Polity.SubPolities.SelectMany(o => o.Agents.Banks).ToList();
    }
}
