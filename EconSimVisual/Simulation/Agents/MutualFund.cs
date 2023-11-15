using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Agents
{
    [Serializable]
    class MutualFund : Business
    {
        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "MutualFund " + id;
        
        public FundType Type { get; set; }

        public MutualFund()
        {

        }
    }

    enum FundType { Equity, Bond, Hybrid }
}
