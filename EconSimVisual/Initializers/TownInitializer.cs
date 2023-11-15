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
        private TownAgents Agents = new TownAgents(null);

        public int Scale { get; set; } = 1;
        public bool CreatesCentralBank { get; set; } = false;
        public bool CreatesGovernment { get; set; } = false;
        public bool CreatesEquityFund { get; set; } = false;

        public TownAgents GetAgents()
        {
            Agents = new TownAgents(null);

            CreatePopulation(1000 * Scale, 10000);
            if (CreatesGovernment)
                CreateGovernment(10000000 * Scale);
            if (CreatesCentralBank)
                CreateCentralBank();
            CreateGrocers(Scale / 2 + 2, 50000);
            //CreateTradeHubs(Scale / 4 + 1, 100000);
            CreateBanks(Scale / 2 + 1, 10000000);
            CreateManufacturers(Scale * 10);
            if (CreatesEquityFund)
                CreateFunds(10000000);
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

        private void CreateTradeHubs(int count, double initialCash)
        {
            for (var i = 0; i < count; i++)
            {
                var hub = new TradeHub()
                {
                    Cash = initialCash
                };
                hub.Manager = new TradeHubManager(hub);
                Agents.TradeHubs.Add(hub);
            }
        }

        private void CreateFunds(double initialCash)
        {
            var fund1 = new MutualFund()
            {
                Type = FundType.Equity,
                Cash = initialCash
            };
            fund1.Manager = new FundManager(fund1);
            Agents.Funds.Add(fund1);

            var fund2 = new MutualFund()
            {
                Type = FundType.Bond,
                Cash = initialCash
            };
            fund2.Manager = new FundManager(fund2);
            Agents.Funds.Add(fund2);

        }

        private void CreateManufacturers(int count)
        {
            for (var i = 0; i < count; i++)
            {
                Agents.AddManufacturer(CreateManufacturer("Potato"));
                Agents.AddManufacturer(CreateManufacturer("Squash"));
                Agents.AddManufacturer(CreateManufacturer("Bread"));
                Agents.AddManufacturer(CreateManufacturer("Wheat"));
                Agents.AddManufacturer(CreateManufacturer("Flour"));
                Agents.AddManufacturer(CreateManufacturer("Beer"));
                Agents.AddManufacturer(CreateManufacturer("Wine"));
                Agents.AddManufacturer(CreateManufacturer("Capital"));
            }
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

        private void CreateCentralBank()
        {
            Agents.CentralBank = new CentralBank();
        }

        private void CreateStocks()
        {
            foreach (var business in Agents.Businesses)
            {
                business.Owners.IssuedStocks.Add(new Stock()
                {
                    IsIssued = true,
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
                    Agents.Banks.GetRandom().Deposits.OpenAccount(business);
            foreach (var person in Agents.Population)
                Agents.Banks.GetRandom().Deposits.OpenAccount(person);
            if (CreatesCentralBank)
                foreach (var bank in Agents.Banks)
                    Agents.CentralBank.Deposits.OpenAccount(bank);
            if (CreatesCentralBank && CreatesGovernment)
                Agents.CentralBank.Deposits.OpenAccount(Agents.Government);
        }

        private void SetInitialPrices()
        {
            foreach (var m in Agents.AllManufacturers.Where(o => o.MainGood == Good.Capital))
                m.Prices[Good.Capital] = 1000;
        }

        private void SetInitialInventories()
        {
            foreach (var m in Agents.AllManufacturers)
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
