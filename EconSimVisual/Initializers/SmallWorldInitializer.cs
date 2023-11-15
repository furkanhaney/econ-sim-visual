using EconSimVisual.Simulation.Polities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Initializers
{
    class SmallWorldInitializer : IWorldInitializer
    {
        public List<Polity> GetPolities()
        {
            var townInitializer = new TownInitializer
            {
                Scale = 5,
                CreatesGovernment = true,
                CreatesCentralBank = true
            };
            return new List<Polity>
            {
                new Town(townInitializer) {Name = "Happyville"}
            };
        }
    }
}
