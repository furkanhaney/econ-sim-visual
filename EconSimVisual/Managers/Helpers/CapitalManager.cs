using System;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Managers.Helpers
{
    internal class CapitalManager : Manager
    {
        public CapitalManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        private Manufacturer Manufacturer { get; }

        public double CapitalReturn
        {
            get
            {
                var price = Manufacturer.Town.Trade.GetLastPrice(Good.Capital);
                var production = Manufacturer.MarginalRevenueCapital;
                var depr = 1 - Manufacturer.CapitalDepreciationRate;

                var depreciation = price - price * Math.Pow(depr, 365);
                var totalProduction = Tools.GetGeometricSum(production, depr, 365);

                return (totalProduction - depreciation) / price;
            }
        }

        public override void Manage()
        {
            while (ShouldGetCapital() && CanShiftCapital())
                Manufacturer.ShiftCapital(1);

            while (ShouldGetCapital() && CanBuyCapital())
                Manufacturer.BuyCapital(1);
        }

        private bool ShouldGetCapital()
        {
            if (Manufacturer.Produces(Good.Capital) && Manufacturer.Goods[Good.Capital] < 200)
                return false;
            return CapitalReturn > 0 &&
                   Manufacturer.NetMoney > 100 * K;
        }

        private bool CanShiftCapital()
        {
            return Manufacturer.Goods[Good.Capital] >= 1;
        }

        private bool CanBuyCapital()
        {
            return Town.Trade.CanBuyGood(Manufacturer, Good.Capital);
        }
    }
}
