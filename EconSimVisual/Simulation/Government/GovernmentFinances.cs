using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Government
{
    [Serializable]
    internal class GovernmentFinances : Entity
    {
        public double CurrentSurplus => Government.Taxes.CurrentRevenue - Government.Welfare.CurrentExpenses;
        public double LastSurplus => Government.Taxes.LastRevenue - Government.Welfare.LastExpenses;
    }
}
