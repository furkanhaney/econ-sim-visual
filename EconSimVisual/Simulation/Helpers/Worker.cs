using EconSimVisual.Simulation.Agents;
using System;

namespace EconSimVisual.Simulation.Helpers
{
    [Serializable]
    internal class Worker
    {
        public Person Person { get; set; }
        public double Wage { get; set; }
        public double UnpaidWages { get; set; }
        public int Tenure { get; set; }
    }
}
