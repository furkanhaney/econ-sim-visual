using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Accounting
{
    internal interface IBalanceSheet
    {
        double TotalAssets { get; }
        double TotalLiabilities { get; }
        double TotalEquity { get; }
    }
}
