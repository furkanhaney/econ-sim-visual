using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using System;

namespace EconSimVisual.Simulation.Information
{
    [Serializable]
    internal class Transaction
    {
        public GoodAmount Goods { get; set; }
        public Entity Buyer { get; set; }
        public Entity Seller { get; set; }
        public double UnitPrice { get; set; }
        public double TaxPaid { get; set; }

        public double TotalPrice => UnitPrice * Goods.Amount;
    }
}