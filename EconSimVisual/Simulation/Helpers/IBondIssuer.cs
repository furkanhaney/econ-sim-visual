using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Helpers
{
    internal interface IBondIssuer
    {
        Bonds Bonds { get; }
    }
}
