namespace EconSimVisual.Simulation.Helpers
{
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