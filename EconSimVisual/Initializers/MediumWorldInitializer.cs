using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Initializers
{
    internal class MediumWorldInitializer : IWorldInitializer
    {
        public int Scale { get; set; } = 1;

        public List<Polity> GetPolities()
        {
            var t1 = new TownInitializer
            {
                Scale = Scale * 2,
                CreatesGovernment = false,
                CreatesCentralBank = false,
                CreatesEquityFund = true
            };
            var t2 = new TownInitializer()
            {
                Scale = Scale,
                CreatesGovernment = false,
                CreatesCentralBank = false
            };
            var cities = new List<Polity>
            {
                new Town(t1) {Name = "Berlin"},
                new Town(t2) {Name = "Dortmund"},
                new Town(t2) {Name = "Hamburg"},
                new Town(t2) {Name = "Hanover"},
                new Town(t2) {Name = "Munich"}
            };
            var germany = new ProperPolity() { Name = "Germany", SubPolities = cities };
            germany.Trade.StockExchange = new SecurityExchange<Stock>();
            germany.Trade.BondExchange = new SecurityExchange<Bond>();
            var gov = germany.Agents.Government = new Government() { Town = (Town)cities[0], Polity = germany, Cash = 10000000 };
            var cb = germany.Agents.CentralBank = new CentralBank() { Town = (Town)cities[0] };
            foreach (var city in cities)
                city.SuperPolity = germany;
            cb.Deposits.OpenAccount(gov);
            foreach (var bank in germany.Agents.Banks)
                cb.Deposits.OpenAccount(bank);

            return new List<Polity>() { germany };
        }
    }
}
