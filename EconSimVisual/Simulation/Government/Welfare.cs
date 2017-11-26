using System.Linq;
using EconSimVisual.Simulation.Government.SocialPrograms;

namespace EconSimVisual.Simulation.Government
{
    internal class Welfare
    {
        public WageProgram UnemploymentWage, UniversalIncome;
        public LowIncomeWage LowIncomeWage;

        public SocialProgram[] AllWelfarePrograms => new SocialProgram[] { UnemploymentWage, UniversalIncome, LowIncomeWage };

        public double CurrentExpenses => AllWelfarePrograms.Sum(o => o.CurrentExpenses);
        public double LastExpenses => AllWelfarePrograms.Sum(o => o.LastExpenses);

        public Welfare()
        {
            UniversalIncome = new UniversalIncome();
            LowIncomeWage = new LowIncomeWage();
            UnemploymentWage = new UnemploymentWage();
        }

        public void Tick()
        {
            UnemploymentWage.Tick();
            UniversalIncome.Tick();
            LowIncomeWage.Tick();
        }

    }
}
