using EconSimVisual.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class ProperPolity : Polity
    {
        public ProperPolity()
        {
            Agents = new ProperPolityAgents(this);
            Trade = new ProperPolityTrade(this);
            Economy = new PolityEconomy(this);
        }

        public List<Polity> SubPolities { get; set; } = new List<Polity>();
        public override List<Polity> AllPolities
        {
            get
            {
                var polities = SubPolities.SelectMany(o => o.AllPolities).ToList();
                polities.Insert(0, this);
                return polities;
            }
        }

        public override void Tick()
        {
            Economy.Tick();
            Trade.FirstTick();
            Agents.Government.FirstTick();
            Agents.CentralBank.FirstTick();

            foreach (var polity in SubPolities)
                polity.Tick();

            Agents.Government.LastTick();
            Agents.CentralBank.LastTick();
            Trade.LastTick();
        }
    }
}