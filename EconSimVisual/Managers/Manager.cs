using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Managers
{
    internal abstract class Manager : Entity, IManager
    {
        public abstract void Manage();
    }
}
