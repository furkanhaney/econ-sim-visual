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
        public List<Worker> Workers { get; set; }
        public double LaborCount => Workers.Count;
        public double LastWagesPaid { get; private set; }
        public double AverageWage
        {
            get { return LaborCount == 0 ? 0 : Workers.Average(o => o.Wage); }
        }
        public double WagesPaid { get; set; }
        public double UnpaidWages => Workers.Sum(o => o.UnpaidWages);

        public void PayWages()
        {
            foreach (var worker in Workers.Shuffle())
            {
                PayWage(worker);
                worker.Tenure++;
            }
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
            // Current wage
            if (Business.CanPay(worker.Wage))
                PayWageAmount(worker, worker.Wage);
            else
                worker.UnpaidWages += worker.Wage;

            // Past wages
            if (worker.UnpaidWages > 0 && Business.CanPay(worker.UnpaidWages))
                PayWageAmount(worker, worker.UnpaidWages);
        }

        private void PayWageAmount(Worker worker, double amount)
        {
            var tax = Taxes.GetAmount(amount, TaxType.Income);
            Business.Pay(worker.Person, amount - tax);
            if (tax > 0)
                Taxes.Pay(Business, tax, TaxType.Income);
            WagesPaid += amount;
        }

        private void RemoveWorker(Worker worker)
        {
            Workers.Remove(worker);
            worker.Person.Workplace = null;
        }
    }
}
