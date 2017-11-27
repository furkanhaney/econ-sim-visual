using System.Reflection;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Polities;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace EconSimVisual.Simulation.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Extensions;
    using Agents;
    using Helpers;

    internal abstract class Entity
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Entity()
        {
            ConsumerGoods.Add(Good.Potato);
            ConsumerGoods.Add(Good.Squash);
            //ConsumerGoods.Add(Good.Bread);
            //ConsumerGoods.Add(Good.Beer);
            ConsumerGoods.Add(Good.Luxury1);
            ConsumerGoods.Add(Good.Luxury2);

            Foods.Add(Good.Potato);
            Foods.Add(Good.Squash);
            Foods.Add(Good.Bread);
        }

        protected const double K = 1000, M = K * K, B = K * K * K;
        private const int PeriodLength = 30;

        public static bool IsPeriodStart => Day % PeriodLength == 0;
        public static int TotalCount { get; set; }
        public static List<Good> ConsumerGoods { get; set; } = new List<Good>();
        public static List<Good> Foods { get; set; } = new List<Good>();

        public static int Day { get; set; }
        protected virtual string DefaultName => string.Empty;
        public string Name => CustomName ?? DefaultName;
        protected virtual string CustomName { get; set; }

        protected static Random Random { get; set; } = new Random();
        public Town Town => SimulationScreen.Town;
        protected Government.Government Government => Town.Agents.Government;
        protected List<Person> Citizens => Town.Agents.Population;
        protected Taxes Taxes => Government.Taxes;

        protected int Id { get; set; } = TotalCount++;

        public static void Assert(bool condition) => Debug.Assert(condition);

        public void Log(string message, LogType type) => Town.TownLogger.Log(message, type);

        public override string ToString() => Name;
    }
}
