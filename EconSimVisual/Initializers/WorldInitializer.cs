using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Initializers
{
    internal class WorldInitializer : IWorldInitializer
    {
        public List<Polity> GetPolities()
        {
            return new List<Polity> { new Town(new TownInitializer()) };
        }
    }
}
