namespace EconSimVisual.Simulation.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    public enum Good { Capital, Potato, Squash, Beer, Wine, Land}

    public class ManufacturingProcess
    {
        private static readonly List<ManufacturingProcess> Processes = GetProcesses();

        public string Name { get; set; }
        public List<GoodAmount> Inputs { get; set; } = new List<GoodAmount>();
        public List<GoodAmount> Outputs { get; set; } = new List<GoodAmount>();
        public double ProductionConstant { get; set; }
        public double LaborConstant { get; set; }
        public double CapitalConstant { get; set; }
        public double LandConstant { get; set; }

        public static ManufacturingProcess Get(Good good)
        {
            return Processes.First(o => o.Outputs.Select(k => k.Good).Contains(good));
        }

        public static ManufacturingProcess Get(string name)
        {
            return Processes.First(o => o.Name.Equals(name));
        }

        public double GetMaximumOutput(double labor, double capital, double land)
        {
            return ProductionConstant * Math.Pow(labor, LaborConstant) * Math.Pow(capital, CapitalConstant) * Math.Pow(land, LandConstant);
        }

        private static List<ManufacturingProcess> GetProcesses()
        {
            return Serializer.Deserialize<List<ManufacturingProcess>>("processes.xml");
        }
    }
}
