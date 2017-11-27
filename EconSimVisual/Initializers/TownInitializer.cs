using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Initializers
{
    internal class TownInitializer : ITownInitializer
    {
        private TownAgents Agents = new TownAgents();

        public TownAgents GetAgents()
        {
            Agents = new TownAgents();

            CreatePopulation(100, 1000);
            CreateGovernment(500000);
            CreateCentralBank(200000);
            CreateBanks(4, 250000);
            CreateGrocers(2, 50000);
            CreateManufacturers();

            CreateStocks();
            CreateBankAccounts();
            SetInitialPrices();

            return Agents;
        }

        private void CreateBanks(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
                Agents.Banks.Add(
                    new CommercialBank()
                    {
                        Cash = initialCash
                    });
        }

        private void CreateGrocers(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
                Agents.Grocers.Add(
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
            Agents.Manufacturers.AddRange(manufacturers);
        }

        private void CreatePopulation(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
            {
                var gender = RandomUtils.GetRandomGender();
                var firstName = RandomUtils.GetRandomFirstName(gender);
                var lastName = RandomUtils.GetRandomLastName();
                Agents.Population.Add(new Person()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender,
                    Cash = initialCash
                });
            }
        }

        private void CreateGovernment(double initialCash)
        {
            Agents.Government = new Government
            {
                Cash = initialCash
            };
        }

        private void CreateCentralBank(double initialCash)
        {
            Agents.CentralBank = new CentralBank
            {
                Cash = initialCash
            };
        }

        private void CreateStocks()
        {
            foreach (var business in Agents.Businesses)
            {
                business.Owners.IssuedStocks.Add(new Stock()
                {
                    Issuer = business,
                    Owner = Agents.Population.GetRandom(),
                    Count = 1000
                });
            }
        }

        private void CreateBankAccounts()
        {
            foreach (var business in Agents.Businesses)
                if (!(business is CommercialBank))
                    Agents.Banks.GetRandom().OpenAccount(business);
            foreach (var person in Agents.Population)
                Agents.Banks.GetRandom().OpenAccount(person);
            foreach (var bank in Agents.Banks)
                Agents.CentralBank.OpenAccount(bank);
        }

        private void SetInitialPrices()
        {
            foreach (var m in Agents.Manufacturers.Where(o => o.Produces(Good.Capital)))
                m.Prices[Good.Capital] = 500;
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

    }
}
