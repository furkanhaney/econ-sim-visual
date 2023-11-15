using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class ProperPolityAgents : PolityAgents
    {
        public ProperPolityAgents(ProperPolity p) : base(p)
        {

        }

        public ProperPolity ProperPolity => Polity as ProperPolity;

        public override List<Person> Population => ProperPolity.SubPolities.SelectMany(o => o.Agents.Population).ToList();
        public override List<Manufacturer> AllManufacturers => ProperPolity.SubPolities.SelectMany(o => o.Agents.AllManufacturers).ToList();
        public override List<Grocer> Grocers => ProperPolity.SubPolities.SelectMany(o => o.Agents.Grocers).ToList();
        public override List<CommercialBank> Banks => ProperPolity.SubPolities.SelectMany(o => o.Agents.Banks).ToList();
        public override List<TradeHub> TradeHubs => ProperPolity.SubPolities.SelectMany(o => o.Agents.TradeHubs).ToList();
        public override List<MutualFund> Funds => ProperPolity.SubPolities.SelectMany(o => o.Agents.Funds).ToList();
        public override Dictionary<Good, List<Manufacturer>> Manufacturers => null;
    }
}
