using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Polities
{
    internal abstract class PolityAgents
    {
        public Government.Government Government { get; set; }
        public CentralBank CentralBank { get; set; }
        public abstract List<Person> Population { get; }
        public abstract List<Manufacturer> Manufacturers { get; }
        public abstract List<Grocer> Grocers { get; }
        public abstract List<CommercialBank> Banks { get; }
        public List<Business> Businesses
        {
            get
            {
                var businesses = new List<Business>();
                businesses.AddRange(Manufacturers.Select(o => (Business)o));
                businesses.AddRange(Grocers.Select(o => (Business)o));
                businesses.AddRange(Banks.Select(o => (Business)o));
                return businesses;
            }
        }
        public List<Agent> All
        {
            get
            {
                var list = new List<Agent>(Businesses);
                list.AddRange(Population);
                list.Add(Government);
                list.Add(CentralBank);
                return list;
            }
        }

        public void Tick()
        {
            FirstTick();
            LastTick();
        }

        private void FirstTick()
        {
            Government.Debt.Adjust();
            foreach (var m in Manufacturers.Shuffle())
                m.FirstTick();
            foreach (var g in Grocers.Shuffle())
                g.FirstTick();
            foreach (var person in Population.Shuffle())
                person.FirstTick();
            foreach (var b in Banks.Shuffle())
                b.FirstTick();
            Government.FirstTick();
        }

        private void LastTick()
        {
            foreach (var m in Manufacturers.Shuffle())
                m.LastTick();
            foreach (var g in Grocers.Shuffle())
                g.LastTick();
            foreach (var person in Population.Shuffle())
                person.LastTick();
            foreach (var b in Banks.Shuffle())
                b.LastTick();
            Government.LastTick();
        }
    }
}