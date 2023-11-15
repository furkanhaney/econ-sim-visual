using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    abstract class Polity
    {
        public string Name { get; set; }
        public Polity SuperPolity { get; set; }
        public PolityAgents Agents { get; protected set; }
        public PolityEconomy Economy { get; protected set; }
        public PolityTrade Trade { get; protected set; }
        public Logger TownLogger { get; }
        
        public Polity()
        {
            TownLogger = new Logger();
        }

        public virtual List<Polity> AllPolities => new List<Polity>() { this };


        public abstract void Tick();

        public override string ToString()
        {
            return Name;
        }
    }
}
