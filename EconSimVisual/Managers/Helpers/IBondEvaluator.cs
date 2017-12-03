using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Managers.Helpers
{
    internal interface IBondEvaluator
    {
        double GetAdjustedYield(Bond bond);
    }
}