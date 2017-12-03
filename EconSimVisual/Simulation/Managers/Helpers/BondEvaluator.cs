using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Managers.Helpers
{
    internal class BondEvaluator : IBondEvaluator
    {
        public double GetAdjustedYield(Bond bond)
        {
            return bond.Yield;
        }
    }
}