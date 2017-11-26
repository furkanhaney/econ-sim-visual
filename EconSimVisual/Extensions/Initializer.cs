using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Town;

namespace EconSimVisual.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Simulation.Agents;
    using Simulation.Helpers;
    using Simulation.Securities;

    internal class Initializer
    {
        private Town town;

        public Town Initialize()
        {
            town = new Town();
            CreateAgents();
            CreateStocks();
            CreateBankAccounts();
            SetInitialPrices();
            return town;
        }

        private static Manufacturer CreateManufacturer(string processName, double initialCash = 50000)
        {
            var m = new Manufacturer(ManufacturingProcess.Get(processName))
            {
                Capital = 100,
                Land = 1,
                Cash = initialCash
            };
            if (!processName.Equals("Capital"))
                m.Goods[m.Process.Outputs[0].Good] = 2000;
            return m;
        }

        private void CreateAgents()
        {
            CreatePopulation(100, 1000);
            CreateGovernment(500000);
            CreateCentralBank(200000);
            CreateBanks(4, 250000);
            CreateGrocers(2, 50000);
            CreateManufacturers();
        }

        private void CreateStocks()
        {
            foreach (var business in town.Agents.Businesses)
            {
                business.Owners.IssuedStocks.Add(new Stock()
                {
                    Issuer = business,
                    Owner = town.Agents.Population.GetRandom(),
                    Count = 1000
                });
            }
        }

        private void CreateBankAccounts()
        {
            foreach (var business in town.Agents.Businesses)
                if (!(business is CommercialBank))
                    town.Agents.Banks.GetRandom().OpenAccount(business);
            foreach (var person in town.Agents.Population)
                town.Agents.Banks.GetRandom().OpenAccount(person);
            foreach (var bank in town.Agents.Banks)
                town.Agents.CentralBank.OpenAccount(bank);
        }

        private void SetInitialPrices()
        {
            foreach (var m in town.Agents.Manufacturers.Where(o => o.Produces(Good.Capital)))
                m.Prices[Good.Capital] = 500;
        }

        private void CreateGovernment(double initialCash)
        {
            town.Agents.Government = new Government
            {
                Cash = initialCash
            };
        }

        private void CreateCentralBank(double initialCash)
        {
            town.Agents.CentralBank = new CentralBank
            {
                Cash = initialCash
            };
        }

        private void CreateBanks(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
                town.Agents.Banks.Add(
            new CommercialBank()
            {
                Cash = initialCash
            });
        }

        private void CreateGrocers(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
                town.Agents.Grocers.Add(
                    new Grocer()
                    {
                        Cash = initialCash
                    });
        }

        private void CreateManufacturers()
        {
            var manufacturers = new List<Manufacturer>()
            {
                CreateManufacturer("Potato"),
                CreateManufacturer("Potato"),
                CreateManufacturer("Squash"),
                CreateManufacturer("Squash"),
                CreateManufacturer("Luxury1"),
                CreateManufacturer("Luxury1"),
                CreateManufacturer("Luxury2"),
                CreateManufacturer("Luxury2"),
                CreateManufacturer("Capital"),
                CreateManufacturer("Capital")
            };
            town.Agents.Manufacturers.AddRange(manufacturers);
        }

        private void CreatePopulation(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
            {
                var gender = RandomUtils.GetRandomGender();
                var firstName = RandomUtils.GetRandomFirstName(gender);
                var lastName = RandomUtils.GetRandomLastName();
                town.Agents.Population.Add(new Person()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender,
                    Cash = initialCash
                });
            }
        }
    }
}
