using EconSimVisual.Simulation.Helpers;
using System;

namespace EconSimVisual.Simulation.Information
{
    [Serializable]
    internal class GoodSummary
    {
        public Good Good { get; set; }
        public double Price { get; set; }
        public double Stocks { get; set; }
        public double Production { get; set; }
        public double Volume { get; set; }
    }
}
