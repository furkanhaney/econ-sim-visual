using System.Collections.Generic;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    internal class World
    {
        public World(IWorldInitializer initializer)
        {
            TopPolities.AddRange(initializer.GetPolities());
        }
        public List<Polity> TopPolities { get; } = new List<Polity>();
        public void Tick()
        {
            Entity.Day++;
            foreach (var polity in TopPolities)
                polity.Tick();
        }
    }
}
