using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Banks
{
    [Serializable]
    internal class BankFinances
    {
        public BankFinances(CommercialBank bank)
        {
            Bank = bank;
        }

        private CommercialBank Bank { get; }
    }
}
