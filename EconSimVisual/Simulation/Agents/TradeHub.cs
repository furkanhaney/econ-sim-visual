using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconSimVisual.Simulation.Agents
{
    [Serializable]
    internal class TradeHub : Vendor
    {
        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "TradeHub " + id;

        public TradeHub() : base(AllGoods)
        {

        }
    }
}
