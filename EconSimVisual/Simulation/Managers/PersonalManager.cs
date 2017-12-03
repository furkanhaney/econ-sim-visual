using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Managers.Helpers;
using MoreLinq;

namespace EconSimVisual.Simulation.Managers
{
    internal class PersonalManager : Manager
    {
        public Person Person { get; }
        public double TargetCash { get; set; } = 2000;

        public PersonalManager(Person person)
        {
            Person = person;
        }

        public override void Manage()
        {
            ManageConsumption();
            ManageEmployment();
            ManageWealth();
        }

        private void ManageWealth()
        {
            if (Person.Cash > TargetCash)
            {
                Person.DepositCash(Person.Cash - TargetCash);
            }
            if (Person.Money > 25000)
            {
                var toBuy = Person.Money - 25000;
                var manager = new BondManager(Person);
                manager.BuyBonds(toBuy);
            }
        }

        private void ManageConsumption()
        {
            SeekFood();
            if (Person.Hunger != 0) return;

            TryConsume(Good.Beer);
        }

        private void ManageEmployment()
        {
            var unemploymentWage = Government.Welfare.UnemploymentWage.Wage;
            if (Person.IsWorking && Person.NetIncome < unemploymentWage * 1.2)
                Person.Workplace.Labor.Quit(Person);
            var jobs = Town.JobsAvailable.Where(j => j.NetPay > 1.2 * unemploymentWage).ToList();

            if (jobs.Count == 0) return;
            var best = jobs.MaxBy(o => o.NetPay);
            if (Person.IsWorking && best.NetPay / Person.GrossIncome > 1.1)
                Person.Workplace.Labor.Quit(Person);
            if (Person.IsWorking) return;
            best.Business.Labor.Hire(Person, best.NetPay);
            Town.JobsAvailable.Remove(best);
        }

        private void SeekFood()
        {
            var canBuy = true;
            var foods = Foods.OrderBy(o => Town.Trade.GetPrice(Person, o));
            while (Person.Hunger > 0 && canBuy)
            {
                canBuy = false;
                foreach (var food in foods)
                    if (Town.Trade.CanBuyGood(Person, food))
                    {
                        Town.Trade.BuyGood(Person, food);
                        canBuy = true;
                        Person.Eat(food);
                        break;
                    }
            }
        }

        private void TryConsume(Good good)
        {
            if (Town.Trade.CanBuyGood(Person, good))
                Town.Trade.BuyGood(Person, good);
            Person.Goods[good] = 0;
        }

    }
}
