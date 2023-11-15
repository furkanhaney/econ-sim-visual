using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EconSimVisual.Extensions;
using EconSimVisual.Managers.Helpers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Polities;
using MoreLinq;

namespace EconSimVisual.Managers
{
    [Serializable]
    internal class PersonalManager : Manager
    {
        public Person Person { get; }
        public int RiskAffinity = Random.Next(5);
        public double TargetCash { get; set; } = 2000;
        public double HappinessToday { get; set; }

        public PersonalManager(Person person)
        {
            Person = person;
        }

        public override void Manage()
        {
            HappinessToday = 0;
            ManageConsumption();
            if (!Person.IsWorking || Person.Hunger > 5 || Random.NextDouble() < 0.05)
                ManageEmployment();
            if (Random.NextDouble() < 0.05)
                ManageWealth();
            Person.Happiness = Person.Happiness * 0.8 + HappinessToday * 0.2;
            if (Person.Hunger > 20 && Day - LastMigration >= 30)
                Migrate();
        }

        private int LastMigration = 0;
        public void Migrate()
        {
            var bestTown = (Person.Town.SuperPolity as ProperPolity).SubPolities.MinBy(o => o.Economy.MaxHunger);
            if (bestTown != Person.Town)
            {
                if (Person.IsWorking)
                    QuitJob();
                Person.Town.Agents.Population.Remove(Person);
                Person.Town = bestTown as Town;
                Person.Town.Agents.Population.Add(Person);
                LastMigration = Day;
            }
        }

        private void ManageWealth()
        {
            if (Person.Cash > TargetCash)
            {
                Person.DepositCash(Person.Cash - TargetCash);
            }

            if (Person.Money >= 25000)
            {
                new SecurityManager(Person).BuyMutualFunds(Person.Money - 25000);
            }
            else if (Person.OwnedSecurities.ToList().Count > 0)
                SellSecurities();
        }

        private void StartBusiness()
        {
            var a = Random.NextDouble();
            if (a < 0.75)
            {
                var process = ManufacturingProcess.Processes.GetRandom();
                var m = new Manufacturer(process);
                m.Manager = new ManufacturerManager(m);
                Person.Pay(m, 25000);
                Person.Town.Agents.AllManufacturers.Add(m);
                m.Owners.IssuedStocks.Add(new Stock()
                {
                    IsIssued = true,
                    Issuer = m,
                    Owner = Person,
                    Count = 1000
                });
            }
            else
            {
                var g = new Grocer();
                g.Manager = new GrocerManager(g);
                Person.Pay(g, 25000);
                Person.Town.Agents.Grocers.Add(g);
                g.Owners.IssuedStocks.Add(new Stock()
                {
                    IsIssued = true,
                    Issuer = g,
                    Owner = Person,
                    Count = 1000
                });
            }
        }

        private void SellSecurities()
        {
            var securities = Person.OwnedSecurities.ToList();
            var exchange = Person.Town.Trade.StockExchange;
            foreach (var security in securities)
                if (security is Stock stock)
                    exchange.All.Remove(stock);
            var toSell = 25000 - Person.Money;
            int i = 0;
            while (toSell >= 0 && securities.Count > i)
            {
                var sec = securities[i];
                if (sec is Stock s)
                {
                    s.UnitPrice = s.Value / s.Count;
                    s.IsOnSale = true;
                    if (toSell > s.Value)
                    {
                        s.OnSaleCount = s.Count;
                        toSell -= s.Value;
                    }
                    else
                    {
                        s.OnSaleCount = (int)(toSell / s.UnitPrice);
                        toSell -= s.OnSaleCount * s.UnitPrice;
                    }
                    if (s.OnSaleCount > 0)
                        exchange.All.Add(s);
                }
                i++;
            }
        }

        private void ManageConsumption()
        {
            //SeekFood();
            Town.TryBuyFood(Person);
            if (Person.Hunger != 0) return;

            HappinessToday += 60;
            if (TryConsume(Good.Beer))
                HappinessToday += 30;
            if (TryConsume(Good.Wine))
                HappinessToday += 10;
        }

        private void ManageEmployment()
        {
            var unemploymentWage = Government.Welfare.UnemploymentWage.Wage;
            if (Person.IsWorking && Person.NetIncome < unemploymentWage * 1.2)
                Person.Workplace.Labor.Quit(Person);
            if (Person.IsWorking)
            {
                var worker = Person.Workplace.Labor.Workers.First(o => o.Person == Person);
                if (Person.Hunger >= 5 && worker.Tenure >= 7)
                    QuitJob();
            }
            var jobs = GetAllJobs();
            if (jobs.Count == 0)
                return;
            var best = jobs.MaxBy(o => o.GrossPay);
            if (Person.IsWorking)
                Person.Workplace.Labor.Quit(Person);
            best.Business.Labor.Hire(Person, best.NetPay);
            best.Positions--;
        }

        private List<JobListing> GetAllJobs()
        {
            var jobs = new List<JobListing>();
            var wageFloor = 1.2 * Math.Max(Person.GrossIncome, 1.2 * Government.Welfare.UnemploymentWage.Wage / (1 - Taxes.Rates[TaxType.Income]));
            foreach (var job in Town.JobsAvailable)
            {
                if (job.Positions > 0 && job.GrossPay >= wageFloor)
                    jobs.Add(job);
            }
            return jobs;
        }

        private void SeekFood()
        {
            var canBuy = true;
            var foods = Foods.OrderBy(o => (Town.Trade as TownTrade).GetPrice(Person, o));
            while (Person.Hunger > 0 && canBuy)
            {
                canBuy = false;
                foreach (var food in foods)
                    if ((Town.Trade as TownTrade).CanBuyGood(Person, food))
                    {
                        (Town.Trade as TownTrade).BuyGood(Person, food);
                        canBuy = true;
                        Person.Eat(food);
                        break;
                    }
            }
        }

        private bool TryConsume(Good good)
        {
            if ((Town.Trade as TownTrade).CanBuyGood(Person, good))
            {
                (Town.Trade as TownTrade).BuyGood(Person, good);
                Person.Goods[good] = 0;
                return true;
            }
            return false;
        }


        private void QuitJob()
        {
            Person.Workplace.Labor.Quit(Person);
        }
    }
}
