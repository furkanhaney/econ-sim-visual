using System.Reflection;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Extensions;
    using Agents;
    using Helpers;
    using EconSimVisual.Simulation.Banks;
    using System.Linq;

    [Serializable]
    internal abstract class Entity
    {
        static Entity()
        {
            Foods.Add(Good.Potato);
            Foods.Add(Good.Squash);
            Foods.Add(Good.Bread);


            ConsumerGoods.AddRange(Foods);
            ConsumerGoods.Add(Good.Beer);
            ConsumerGoods.Add(Good.Wine);
        }

        protected const double K = 1000, M = K * K, B = K * K * K;
        private const int PeriodLength = 30;

        public static bool IsPeriodStart => Day % PeriodLength == 0;
        public static int TotalCount { get; set; }
        public static List<Good> ConsumerGoods { get; set; } = new List<Good>();
        public static List<Good> Foods { get; set; } = new List<Good>();
        public static List<Good> AllGoods
        {
            get
            {
                return Enum.GetValues(typeof(Good)).Cast<Good>().ToList();
            }
        }
        public static Random Rnd = new Random();

        public static int Day { get; set; }
        protected virtual string DefaultName => string.Empty;
        public string Name => CustomName ?? DefaultName;
        protected virtual string CustomName { get; set; }

        protected static Random Random { get; set; } = new Random();
        public virtual Town Town { get; set; }
        public virtual Polity Polity { get; set; }
        public Government.Government Government
        {
            get
            {
                if (Town.Agents.Government == null)
                    return Town.SuperPolity.Agents.Government;
                return Town.Agents.Government;
            }
        }
        public CentralBank CentralBank
        {
            get
            {
                if (Town.Agents.CentralBank == null)
                    return Town.SuperPolity.Agents.CentralBank;
                return Town.Agents.CentralBank;
            }
        }
        protected List<Person> Citizens => Town.Agents.Population;
        protected Taxes Taxes => Government.Taxes;

        protected int Id { get; set; } = TotalCount++;

        public static void Assert(bool condition) => Debug.Assert(condition);

        public void Log(string message, LogType type) => Town.TownLogger.Log(message, type);

        public override string ToString() => Name;
    }
}
