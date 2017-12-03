using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Managers.Helpers
{
    internal class BondEvaluator : IBondEvaluator
    {
        public double GetAdjustedYield(Bond bond)
        {
            return bond.Yield;
        }
    }
}