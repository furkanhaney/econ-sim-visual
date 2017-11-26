using System;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Government.SocialPrograms
{
    internal class LowIncomeWage : WageProgram
    {
        public double Threshold { get; set; }
        public override Func<Person, bool> Qualifier => (p => p.NetIncome < Threshold);
    }
}
