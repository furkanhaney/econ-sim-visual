using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Polities;
using System;

namespace EconSimVisual.Managers
{
    [Serializable]
    internal abstract class Manager : Entity, IManager
    {
        public abstract void Manage();
    }
}
