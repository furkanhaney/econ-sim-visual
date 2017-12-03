using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EconSimVisual.Extensions;
using EconSimVisual.Managers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Polities;

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
            CreateBanks(1, 1000000);
            CreateGrocers(2, 50000);
            CreateManufacturers();

            CreateStocks();
            CreateBankAccounts();
            SetInitialPrices();
            SetInitialInventories();

            return Agents;
        }

        private void CreateBanks(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
            {
                var bank = new CommercialBank
                {
                    Cash = initialCash
                };
                bank.Manager = new CommercialBankManager(bank);
                Agents.Banks.Add(bank);
            }
        }

        private void CreateGrocers(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
            {
                var grocer = new Grocer()
                {
                    Cash = initialCash
                };
                grocer.Manager = new GrocerManager(grocer);
                Agents.Grocers.Add(grocer);
            }
        }

        private void CreateManufacturers()
        {
            var manufacturers = new List<Manufacturer>()
            {
                CreateManufacturer("Potato"),
                CreateManufacturer("Squash"),
                CreateManufacturer("Beer"),
                CreateManufacturer("Wine"),
                CreateManufacturer("Capital")
            };
            Agents.Manufacturers.AddRange(manufacturers);
        }

        private void CreatePopulation(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
                Agents.Population.Add(CreatePerson(initialCash));
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
            Agents.CentralBank.OpenAccount(Agents.Government);
        }

        private void SetInitialPrices()
        {
            foreach (var m in Agents.Manufacturers.Where(o => o.Produces(Good.Capital)))
                m.Prices[Good.Capital] = 1000;
        }

        private void SetInitialInventories()
        {
            foreach (var m in Agents.Manufacturers)
                foreach (var good in m.Process.Outputs.Concat(m.Process.Inputs))
                    m.Goods[good.Good] = 10 + 1000 * m.Process.ProductionConstant;
        }

        private static Manufacturer CreateManufacturer(string processName, double initialCash = 50000)
        {
            var m = new Manufacturer(ManufacturingProcess.Get(processName))
            {
                Capital = 500,
                Land = 1,
                Cash = initialCash
            };
            m.Manager = new ManufacturerManager(m);
            return m;
        }

        private static Person CreatePerson(double initialCash)
        {
            var gender = RandomUtils.GetRandomGender();
            var person = new Person
            {
                FirstName = RandomUtils.GetRandomFirstName(gender),
                LastName = RandomUtils.GetRandomLastName(),
                Gender = gender,
                Cash = initialCash
            };
            person.Manager = new PersonalManager(person);
            return person;
        }
    }
}
