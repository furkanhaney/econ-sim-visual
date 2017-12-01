using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Government
{
    internal class Government : Agent, IBondIssuer
    {
        public Government()
        {
            TargetCash = 250000;
            Taxes = new Taxes();
            Welfare = new Welfare();
            Bonds = new Bonds(this);
            Finances = new GovernmentFinances();
        }

        protected override string DefaultName => "Government";
        public new Taxes Taxes { get; }
        public Welfare Welfare { get; }
        public Bonds Bonds { get; }
        public GovernmentFinances Finances { get; }

        public override void FirstTick()
        {
            if (Bonds.Exchange == null)
                Bonds.Exchange = Town.Trade.BondExchange;
        }
        public override void LastTick()
        {
            Taxes.LastTick();
            Welfare.Tick();
            base.LastTick();
        }
    }
}
