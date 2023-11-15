using EconSimVisual.Simulation.Instruments.Securities;
using System;

namespace EconSimVisual.Managers.Helpers
{
    [Serializable]
    internal class BondEvaluator : IBondEvaluator
    {
        public double GetAdjustedYield(Bond bond)
        {
            return bond.Yield;
        }
    }
}