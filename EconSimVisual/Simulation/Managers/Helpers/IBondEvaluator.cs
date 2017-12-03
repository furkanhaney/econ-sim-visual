using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Managers.Helpers
{
    internal interface IBondEvaluator
    {
        double GetAdjustedYield(Bond bond);
    }
}