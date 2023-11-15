using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class TownAgents : PolityAgents
    {

        public TownAgents(Town p) : base(p)
        {
            manufacturers = new Dictionary<Good, List<Manufacturer>>();
            foreach (var good in Entity.AllGoods)
                manufacturers.Add(good, new List<Manufacturer>());
        }

        public void AddManufacturer(Manufacturer m)
        {
            AllManufacturers.Add(m);
            Manufacturers[m.MainGood].Add(m);
        }

        private Dictionary<Good, List<Manufacturer>> manufacturers;
        public override Dictionary<Good, List<Manufacturer>> Manufacturers => manufacturers;
        public override List<Person> Population { get; } = new List<Person>();
        public override List<Manufacturer> AllManufacturers { get; } = new List<Manufacturer>();
        public override List<Grocer> Grocers { get; } = new List<Grocer>();
        public override List<CommercialBank> Banks { get; } = new List<CommercialBank>();
        public override List<TradeHub> TradeHubs { get; } = new List<TradeHub>();
        public override List<MutualFund> Funds { get; } = new List<MutualFund>();
    }
}