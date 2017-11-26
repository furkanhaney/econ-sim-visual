namespace EconSimVisual.Simulation.Agents
{
    using System;
    using System.Linq;
    using Helpers;
    using Managers;

    /// <summary>
    ///     Business that engages in the production of goods.
    /// </summary>
    internal class Manufacturer : Vendor
    {
        public const double CapitalDepreciationRate = 0.002;

        public Manufacturer(ManufacturingProcess process) : base(process.Outputs.Select(o => o.Good).ToList())
        {
            Process = process;
            Manager = new ManufacturerManager(this);
        }

        public double Capital { get; set; }
        public double CapitalValue => Capital * Town.Trade.GetLastPrice(Good.Capital);
        public double CapitalDepreciation => CapitalValue * CapitalDepreciationRate;
        public override double Assets => BaseAssets + CapitalValue;
        protected override string DefaultName => Process.Name + " Manufacturer";
        public ManufacturingProcess Process { get; set; }
        public double MaximalOutput => Process.GetMaximumOutput(Labor.LaborCount, Capital, Land);
        public double ActualOutput { get; set; }
        public double MarginalProfit
        {
            get
            {
                var revenues = Process.Outputs.Sum(o => o.Amount * Prices[o.Good]);
                var expenses = Process.Inputs.Sum(o => o.Amount * Town.Trade.GetLastPrice(o.Good));
                return revenues - expenses;
            }
        }
        public double MarginalRevenueLabor
        {
            get
            {
                var marginalProduct = Process.GetMaximumOutput(Labor.LaborCount + 1, Capital, Land) - MaximalOutput;
                return marginalProduct * MarginalProfit;
            }
        }
        public double MarginalRevenueCapital
        {
            get
            {
                var marginalProduct = Process.GetMaximumOutput(Labor.LaborCount, Capital + 1, Land) - MaximalOutput;
                return marginalProduct * MarginalProfit;
            }
        }
        public double MarginalRevenueLand
        {
            get
            {
                var marginalProduct = Process.GetMaximumOutput(Labor.LaborCount, Capital, Land + 1) - MaximalOutput;
                return marginalProduct * MarginalProfit;
            }
        }

        public override void FirstTick()
        {
            base.FirstTick();
            Produce();
        }

        public override void LastTick()
        {
            base.LastTick();
            Capital *= 1 - CapitalDepreciationRate;
        }

        public bool Produces(Good good)
        {
            return Process.Outputs.Any(o => o.Good == good);
        }

        public void BuyCapital(double amount)
        {
            Town.Trade.BuyGood(this, Good.Capital, amount);
        }

        public void ShiftCapital(double amount)
        {
            Capital += amount;
            Goods[Good.Capital] -= amount;
            Assert(Goods[Good.Capital] >= 0);
        }

        private void Produce()
        {
            var output = MaximalOutput;
            foreach (var input in Process.Inputs)
                output = Math.Min(output, Goods[input.Good] / input.Amount);
            ActualOutput = output;
            foreach (var i in Process.Inputs)
                Goods[i.Good] -= i.Amount * output;
            foreach (var o in Process.Outputs)
                Goods[o.Good] += o.Amount * output;
        }
    }
}