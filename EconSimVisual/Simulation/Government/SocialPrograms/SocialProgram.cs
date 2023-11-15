using EconSimVisual.Simulation.Base;
using System;

namespace EconSimVisual.Simulation.Government.SocialPrograms
{
    [Serializable]
    internal abstract class SocialProgram : Entity
    {
        public double LastExpenses { get; set; }
        public double CurrentExpenses { get; set; }

        public virtual void Tick()
        {
            if (IsPeriodStart)
                ResetExpenses();
        }

        private void ResetExpenses()
        {
            LastExpenses = CurrentExpenses;
            CurrentExpenses = 0;
        }
    }
}
