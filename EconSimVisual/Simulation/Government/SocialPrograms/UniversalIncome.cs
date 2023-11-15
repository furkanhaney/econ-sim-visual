using System;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Government.SocialPrograms
{
    [Serializable]
    internal class UniversalIncome : WageProgram
    {
        public override Func<Person, bool> Qualifier => (p => true);
    }
}
