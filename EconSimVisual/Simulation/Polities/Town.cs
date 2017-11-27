using System.Collections.Generic;
using System.Reflection;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using log4net;

namespace EconSimVisual.Simulation.Polities

{
    // Smallest political structure
    internal class Town : Polity
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Town(ITownInitializer initializer)
        {
            Agents = initializer.GetAgents();
            Economy = new PolityEconomy(this);
            Trade = new TradeManager(this);
            TownLogger = new Logger();
        }

        public Logger TownLogger { get; }
        public TradeManager Trade { get; }

        public List<JobListing> JobsAvailable { get; set; } = new List<JobListing>();

        public override void Tick()
        {
            Trade.FirstTick();
            base.Tick();
            Trade.LastTick();
        }
    }
}
