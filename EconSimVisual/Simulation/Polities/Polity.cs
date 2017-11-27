using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    internal class Polity
    {
        public PolityAgents Agents { get; protected set; }
        public PolityEconomy Economy { get; protected set; }
        public virtual void Tick()
        {
            Agents.Tick();
            Economy.Tick();
        }
    }
}
