using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Managers.Helpers
{
    internal class CapitalManager : BusinessManager
    {
        public CapitalManager(Manufacturer manufacturer)
        {
            Manufacturer = manufacturer;
        }

        private Manufacturer Manufacturer { get; }

        public override void Manage()
        {
            var cost = CalculateCostOfCapital();

            while (ShouldGetCapital(cost) && CanShiftCapital())
                Manufacturer.ShiftCapital(1);

            while (ShouldGetCapital(cost) && CanBuyCapital())
                Manufacturer.BuyCapital(1);
        }

        private bool ShouldGetCapital(double cost)
        {
            return Manufacturer.MarginalRevenueCapital > cost &&
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

        private double CalculateCostOfCapital()
        {
            var cost = Manufacturer.CapitalDepreciationRate;
            if (Manufacturer.BankAccounts.Any())
                cost += Manufacturer.BankAccounts.Max(o => o.SavingsRate);
            return cost * Town.Trade.GetLastPrice(Good.Capital);
        }
    }
}
