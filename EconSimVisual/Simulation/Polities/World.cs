using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal class World
    {
        public int Day { get; set; }

        public World(IWorldInitializer initializer)
        {
            TopPolities.AddRange(initializer.GetPolities());
        }
        public List<Polity> TopPolities { get; } = new List<Polity>();
        public List<Polity> AllPolities
        {
            get
            {
                return TopPolities.SelectMany(o => o.AllPolities).ToList();
            }
        }

        public void Tick()
        {
            Entity.Day++;
            foreach (var polity in TopPolities)
                polity.Tick();
        }
    }
}
