using System;
using System.Linq;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government.SocialPrograms;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Government
{
    [Serializable]
    internal class Welfare : Entity
    {
        public WageProgram UnemploymentWage, UniversalIncome;
        public LowIncomeWage LowIncomeWage;

        public SocialProgram[] AllWelfarePrograms => new SocialProgram[] { UnemploymentWage, UniversalIncome, LowIncomeWage };

        public double CurrentExpenses => AllWelfarePrograms.Sum(o => o.CurrentExpenses);
        public double LastExpenses => AllWelfarePrograms.Sum(o => o.LastExpenses);

        public override Town Town
        {
            get => base.Town; set
            {
                base.Town = value;
                foreach (var prg in AllWelfarePrograms)
                    prg.Town = value;
            }
        }

        public override Polity Polity
        {
            get => base.Polity; set
            {
                base.Polity = value;
                foreach (var prg in AllWelfarePrograms)
                    prg.Polity = value;
            }
        }

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
