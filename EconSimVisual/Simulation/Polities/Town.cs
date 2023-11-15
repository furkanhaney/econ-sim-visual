using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Polities

{
    [Serializable]
    // Smallest political structure
    internal class Town : Polity
    {
        public Town(ITownInitializer initializer)
        {
            Economy = new PolityEconomy(this);
            Trade = new TownTrade(this);
            Agents = initializer.GetAgents();
            Agents.Polity = this;
            foreach (var a in Agents.All)
            {
                a.Town = this;
                if (!(a.Manager is null))
                    a.Manager.Town = this;
            }

        }

        public List<JobListing> JobsAvailable { get; set; } = new List<JobListing>();

        public override void Tick()
        {
            Trade.FirstTick();
            Agents.Tick();
            Trade.LastTick();
            Economy.Tick();
        }

        private int lastUpdated = int.MinValue;
        public List<FoodInfo> Foods = new List<FoodInfo>();

        public void SortJobs()
        {
            JobsAvailable = JobsAvailable.OrderByDescending(o => o.NetPay).ToList();
        }

        public void TryBuyFood(Person person)
        {
            if (Entity.Day > lastUpdated)
            {
                lastUpdated = Entity.Day;
                Foods.Clear();
                foreach (var food in Entity.Foods)
                    foreach (var grocer in Agents.Grocers)
                        Foods.Add(new FoodInfo()
                        {
                            Good = food,
                            Seller = grocer,
                            Price = grocer.Prices[food]
                        });
                Foods = Foods.OrderBy(o => o.Price).ToList();
            }
            while (Foods.Count > 0 && Foods[0].Seller.Goods[Foods[0].Good] <= 1)
                Foods.RemoveAt(0);
            if (Foods.Count == 0)
                return;
            if (person.CanPay(Foods[0].Price) && person.Hunger > 0)
            {
                (Trade as TownTrade).BuyGood(person, Foods[0].Seller, Foods[0].Good, 1);
                if (person.Goods[Foods[0].Good] >= 1)
                    person.Eat(Foods[0].Good);
            } else
                return;
            if (person.Hunger > 0)
                TryBuyFood(person);
        }

        private double GetLowestFoodPrice(Grocer grocer)
        {
            return grocer.Prices.Where(j => Entity.Foods.Contains(j.Key)).Min(o => o.Value);
        }
    }

    [Serializable]
    struct FoodInfo
    {
        public Grocer Seller { get; set; }
        public Good Good { get; set; }
        public double Price { get; set; }
    }
}
