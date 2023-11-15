using System;

namespace EconSimVisual.Simulation.Helpers
{
    [Serializable]
    /// <summary>
    /// Immutable
    /// </summary>
    public class GoodAmount
    {
        public Good Good { get; set; }
        public double Amount { get; set; }

        public GoodAmount(Good good, double amount)
        {
            Good = good;
            Amount = amount;
        }

        public GoodAmount()
        {

        }
    }
}