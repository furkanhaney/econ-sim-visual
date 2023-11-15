using EconSimVisual.Simulation.Polities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Agents
{
    internal interface IManager
    {
        Town Town { get; set; }
        void Manage();
    }
}
