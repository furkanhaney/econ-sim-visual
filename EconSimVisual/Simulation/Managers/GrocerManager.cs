namespace EconSimVisual.Simulation.Managers
{
    using Extensions;
    using Agents;

    internal class GrocerManager : BusinessManager
    {
        public GrocerManager(Grocer grocer)
        {
            Grocer = grocer;
        }

        private Grocer Grocer { get; }
        private double Margin { get; set; }

        public override void Manage()
        {
            Margin = Random.NextDouble(0.05, 0.10);
            ManagePrices();
            ManageStocks();
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
