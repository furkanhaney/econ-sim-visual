using System.Collections.Generic;
using System.Linq;

namespace EconSimVisual.Simulation.Instruments.Loans
{
    internal class Loans
    {
        public Loans()
        {
            MadeLoans = new List<SimpleLoan>();
        }
        public List<SimpleLoan> MadeLoans { get; }

        public void Tick()
        {
            foreach (var loan in MadeLoans.ToList())
            {
                loan.MaturityDays--;
            }
        }


    }
}
