using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Information
{
    internal class GoodSummary
    {
        public Good Good { get; set; }
        public double Price { get; set; }
        public double PriceChange { get; set; }
        public double Stocks { get; set; }
        public double Production { get; set; }
        public double Volume { get; set; }
    }
}
