using System.Collections.Generic;

namespace EconSimVisual.Simulation.Polities
{
    internal class World
    {
        public World(IWorldInitializer initializer)
        {
            TopPolities.AddRange(initializer.GetPolities());
        }
        public int Day { get; private set; }
        public List<Polity> TopPolities { get; } = new List<Polity>();
        public void Tick()
        {
            Day++;
            foreach (var polity in TopPolities)
                polity.Tick();
        }
    }
}
