using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Polities
{
    [Serializable]
    class ProperPolityEconomy : PolityEconomy
    {
        public ProperPolityEconomy(ProperPolity polity) : base(polity)
        {
            Polity = polity;
        }


        public override double NominalGdp => 0;
    }
}
