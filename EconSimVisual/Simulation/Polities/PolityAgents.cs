using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;
using System;
using System.Windows;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    internal abstract class PolityAgents
    {
        public PolityAgents(Polity polity)
        {
            Polity = polity;
        }

        public Polity Polity { get; set; }
        public Government.Government Government { get; set; }
        public CentralBank CentralBank { get; set; }
        public abstract List<Person> Population { get; }
        public abstract List<Manufacturer> AllManufacturers { get; }
        public abstract Dictionary<Good, List<Manufacturer>> Manufacturers { get; }
        public abstract List<Grocer> Grocers { get; }
        public abstract List<TradeHub> TradeHubs { get; }
        public abstract List<CommercialBank> Banks { get; }
        public abstract List<MutualFund> Funds { get; }
        public List<Business> Businesses
        {
            get
            {
                var businesses = NonBankBusinesses;
                businesses.AddRange(Banks.Select(o => (Business)o));
                return businesses;
            }
        }
        public List<Business> NonBankBusinesses
        {
            get
            {
                var businesses = new List<Business>();
                businesses.AddRange(AllManufacturers.Select(o => (Business)o));
                businesses.AddRange(Grocers.Select(o => (Business)o));
                businesses.AddRange(TradeHubs.Select(o => (Business)o));
                businesses.AddRange(Funds.Select(o => (Business)o));
                return businesses;
            }
        }
        public List<Agent> All
        {
            get
            {
                var list = new List<Agent>(Businesses);
                list.AddRange(Population);
                if (Government != null)
                    list.Add(Government);
                if (CentralBank != null)
                    list.Add(CentralBank);
                return list;
            }
        }

        public List<T> GetBusinesses<T>() where T : Business
        {
            return Businesses.Where(o => o is T).Cast<T>().ToList();
        }

        public void Tick()
        {
            FirstTick();
            LastTick();
        }

        private void FirstTick()
        {
            if (CentralBank != null)
                CentralBank.FirstTick();
            foreach (var h in TradeHubs.Shuffle())
                h.FirstTick();
            foreach (var m in AllManufacturers.Shuffle())
                m.FirstTick();
            foreach (var g in Grocers.Shuffle())
                g.FirstTick();
            if (Polity is Town t)
                t.SortJobs();
            foreach (var person in Population.Shuffle())
                person.FirstTick();
            foreach (var f in Funds.Shuffle())
                f.FirstTick();
            foreach (var b in Banks.Shuffle())
                b.FirstTick();
            if (Government != null)
                Government.FirstTick();
        }

        private void LastTick()
        {
            foreach (var m in AllManufacturers.Shuffle())
                m.LastTick();
            foreach (var g in Grocers.Shuffle())
                g.LastTick();
            foreach (var person in Population.Shuffle())
                person.LastTick();
            foreach (var b in Banks.Shuffle())
                b.LastTick();
            foreach (var h in TradeHubs.Shuffle())
                h.LastTick();
            foreach (var f in Funds.Shuffle())
                f.LastTick();
            if (Government != null)
                Government.LastTick();
        }
    }
}