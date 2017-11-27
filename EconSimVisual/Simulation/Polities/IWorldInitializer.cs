using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Polities
{
    internal interface IWorldInitializer
    {
        List<Polity> GetPolities();
    }
}
