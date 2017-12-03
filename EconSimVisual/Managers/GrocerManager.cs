using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;

namespace EconSimVisual.Managers
{
    internal class GrocerManager : Manager
    {
        public GrocerManager(Grocer grocer)
        {
            Grocer = grocer;
        }

        private Grocer Grocer { get; }
        private double Margin { get; set; }

        public override void Manage()
        {
            Margin = Random.NextDouble(0.10, 0.15);
            ManageFunds();
            ManagePrices();
            ManageStocks();
        }

        private void ManageFunds()
        {
            if (Grocer.Cash > 0)
                Grocer.DepositCash(Grocer.Cash);
        }

        private void ManagePrices()
        {
            foreach (var good in ConsumerGoods)
            {
                var marketPrice = Town.Trade.GetLastPrice(good);
                Grocer.Prices[good] = marketPrice * (1 + Margin);
            }
        }

        // TODO Improve this
        private void ManageStocks()
        {
            foreach (var good in ConsumerGoods)
                Grocer.TargetStocks[good] = 2000;
        }
    }
}
