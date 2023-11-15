using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using System;

namespace EconSimVisual.Managers
{
    [Serializable]
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
            Margin = Random.NextDouble(1.00, 2.00);
            ManageFunds();
            ManagePrices();
            ManageStocks();
        }

        private void ManageFunds()
        {
            if (Grocer.Cash > 0)
                Grocer.DepositCash(Grocer.Cash);

            var current = Grocer.Bonds.Current;
            current.MaturityDays = 365;
            current.FaceValue = 1000;
            current.UnitPrice = 900;
            if (Grocer.Money < 25000)
                current.Count = current.OnSaleCount = 5;
            else
                current.Count = current.OnSaleCount = 0;

        }

        private void ManagePrices()
        {
            foreach (var good in ConsumerGoods)
            {
                var marketPrice = (Town.Trade as TownTrade).GetUnitPrice(good);
                Grocer.Prices[good] = marketPrice * (1 + Margin);
            }
        }

        private void ManageStocks()
        {
            Grocer.TargetStocks[Good.Potato] = 10000;
            Grocer.TargetStocks[Good.Squash] = 10000;
            Grocer.TargetStocks[Good.Bread] = 10000;
            Grocer.TargetStocks[Good.Beer] = 1000;
            Grocer.TargetStocks[Good.Wine] = 100;
        }
    }
}
