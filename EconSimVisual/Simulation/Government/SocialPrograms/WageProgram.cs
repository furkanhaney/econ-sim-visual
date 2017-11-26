using System;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Simulation.Government.SocialPrograms
{
    internal abstract class WageProgram : SocialProgram
    {
        public double Wage { get; set; }
        public abstract Func<Person, bool> Qualifier { get; }

        public override void Tick()
        {
            if (Wage != 0)
                MakePayments();
            base.Tick();
        }

        private void MakePayments()
        {
            foreach (var person in Citizens.Where(o => Qualifier(o)))
                if (Government.CanPay(Wage))
                {
                    Government.Pay(person, Wage);
                    CurrentExpenses += Wage;
                }
                else
                    Town.TownLogger.Log("Government could not pay " + person + " their wage.", LogType.NonPayment);
        }
    }
}
