using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Agents
{
    using System.Linq;

    using Base;
    using Helpers;
    using Securities;

    using MoreLinq;

    internal class Person : Agent
    {
        public Person()
        {
            TargetCash = 5000;
        }

        protected override string CustomName => FirstName + " " + LastName;
        public int Hunger { get; set; }
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
        public double Dividends => OwnedAssets.Where(a => a is Stock).Sum(a => ((Stock)a).Dividends);
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

            ManageConsumption();
            ManageEmployment();
            base.FirstTick();
        }

        private void UpdateStats()
        {
            Hunger++;
        }

        private void ManageConsumption()
        {
            SeekFood();
            if (Hunger != 0) return;

            TryConsume(Good.Beer);
        }

        private void TryConsume(Good good)
        {
            if (Town.Trade.CanBuyGood(this, good))
                Town.Trade.BuyGood(this, good);
            Goods[good] = 0;
        }

        private void ManageEmployment()
        {
            var unemploymentWage = Government.Welfare.UnemploymentWage.Wage;
            if (IsWorking && NetIncome < unemploymentWage * 1.2)
                Workplace.Labor.Quit(this);
            var jobs = Town.JobsAvailable.Where(j => j.NetPay > 1.2 * unemploymentWage).ToList();

            if (jobs.Count == 0) return;
            var best = jobs.MaxBy(o => o.NetPay);
            if (IsWorking && best.NetPay / GrossIncome > 1.1)
                Workplace.Labor.Quit(this);
            if (IsWorking) return;
            best.Business.Labor.Hire(this, best.NetPay);
            Town.JobsAvailable.Remove(best);
        }

        private void SeekFood()
        {
            var canBuy = true;
            var foods = Foods.OrderBy(o => Town.Trade.GetPrice(this, o));
            while (Hunger > 0 && canBuy)
            {
                canBuy = false;
                foreach (var food in foods)
                    if (Town.Trade.CanBuyGood(this, food))
                    {
                        Town.Trade.BuyGood(this, food);
                        canBuy = true;
                        Goods[food]--;
                        Hunger--;
                        break;
                    }
            }
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
