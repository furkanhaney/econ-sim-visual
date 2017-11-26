using System.Collections.Generic;
using System.Reflection;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using log4net;

namespace EconSimVisual.Simulation.Town
{
    internal class Town : Entity
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Town()
        {
            Agents = new AgentManager(this);
            Economy = new EconomyTracker(this);
            Trade = new TradeManager(this);
            TownLogger = new Logger();
        }

        public static Town Current { get; set; }

        public Logger TownLogger { get; }
        public AgentManager Agents { get; }
        public EconomyTracker Economy { get; }
        public TradeManager Trade { get; }

        public List<JobListing> JobsAvailable { get; set; } = new List<JobListing>();

        public void Tick()
        {
            log.Debug("Tick()");
            Day++;
            Trade.FirstTick();
            Agents.Tick();
            Trade.LastTick();
            Economy.Tick();
        }
    }
}
