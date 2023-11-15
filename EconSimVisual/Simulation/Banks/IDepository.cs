using System.Collections.Generic;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Banks
{
    internal interface IDepository
    {
        Deposits Deposits { get; }
    }
}
