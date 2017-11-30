using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Helpers
{
    internal class Labor : Entity
    {
        public static void Test()
        {

        }
        public Labor(Business business)
        {
            Business = business;
            Workers = new List<Worker>();
        }

        public Business Business { get; }
        public bool IsHiring { get; set; }
        public double BaseWage { get; set; } = 10;
        public List<Worker> Workers { get; set; }
        public double LaborCount => Workers.Count;
        public double LastWagesPaid { get; private set; }
        public double CurrentWages => Workers.Sum(o => o.Wage);
        public double AverageWage
        {
            get { return LaborCount == 0 ? 0 : Workers.Average(o => o.Wage); }
        }
        public double WagesPaid { get; set; }

        public void PayWages()
        {
            foreach (var worker in Workers)
                PayWage(worker);
        }

        public void ResetStats()
        {
            LastWagesPaid = WagesPaid;
            WagesPaid = 0;
        }

        public void Hire(Person person, double wage)
        {
            Log(Business + " hired " + person, LogType.Hiring);
            Workers.Add(new Worker
            {
                Person = person,
                Wage = wage
            });
            person.Workplace = Business;
        }

        public void Fire(Worker worker)
        {
            RemoveWorker(worker);
            Log(Business + " fired " + worker.Person + ".", LogType.Hiring);

        }

        public void Quit(Person person)
        {
            RemoveWorker(Workers.First(o => o.Person == person));
            Log(person + " quit " + Business + ".", LogType.Hiring);
        }

        private void PayWage(Worker worker)
        {
            var wage = worker.Wage;
            var tax = Taxes.GetAmount(worker.Wage, TaxType.Income);
            if (Business.CanPay(wage))
            {
                Business.Pay(worker.Person, wage);
                if (tax > 0)
                    Taxes.Pay(Business, tax, TaxType.Income);
                WagesPaid += wage;
            }
            else
                Log(this + " has defaulted on paying wages.", LogType.NonPayment);
            worker.Tenure++;
        }

        private void RemoveWorker(Worker worker)
        {
            Workers.Remove(worker);
            worker.Person.Workplace = null;
        }
    }
}
