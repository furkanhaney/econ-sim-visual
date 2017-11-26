using System;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Government.SocialPrograms
{
    internal class UnemploymentWage : WageProgram
    {
        public override Func<Person, bool> Qualifier => (p => !p.IsWorking);
    }
}
