using EconSimVisual.Extensions;
using EconSimVisual.Managers;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Agents
{
    using System.Linq;

    using Base;
    using Helpers;
    using MoreLinq;

    internal class Person : Agent
    {
        public Person()
        {
        }

        protected override string CustomName => FirstName + " " + LastName;
        public int Hunger { get; set; }
        public double Happiness { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public Business Workplace { get; set; }
        public bool IsWorking => Workplace != null;
        public double GrossIncome => Salary + Dividends;
        public double NetIncome
        {
            get
            {
                var netSalary = Salary * (1 - Taxes.Rates[TaxType.Income]);
                var netDividends = Dividends * (1 - Taxes.Rates[TaxType.Dividend]);
                return netSalary + netDividends;
            }
        }
        public double Dividends => OwnedAssets.Where(a => a is Stock).Sum(a => ((Stock)a).TotalDividends);
        public double Salary => Workplace?.Labor.Workers.First(o => o.Person == this).Wage ?? 0;
        public double NetWorth => BalanceSheet.TotalEquity;

        public override void FirstTick()
        {
            UpdateStats();
            if (Hunger > 40)
            {
                Die("starvation");
                return;
            }
            base.FirstTick();
        }

        public void Eat(Good good)
        {
            Goods[good]--;
            Hunger--;
        }

        private void UpdateStats()
        {
            Hunger++;
        }

        private void Die(string cause)
        {
            Log(this + " has died of " + cause + "!", LogType.Death);
            if (IsWorking)
                Workplace.Labor.Quit(this);
            Town.Agents.Population.Remove(this);
            HandleInheritance();
        }

        private void HandleInheritance()
        {
            foreach (var asset in OwnedAssets.ToList())
                asset.Owner = Government;
            foreach (var account in BankAccounts.ToList())
            {
                if (account.Balance > 0)
                    account.Withdraw(account.Balance);
                account.Close();
            }
            foreach (var good in EnumUtils.GetValues<Good>())
                if (Goods[good] > 0)
                    Transfer(Government, good, Goods[good]);

            PayCash(Government, Cash);
        }
    }

    public enum Gender { Male, Female }
}
