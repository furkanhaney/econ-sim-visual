using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Instruments.Securities;

namespace EconSimVisual.Simulation.Government
{
    internal class Government : Agent, IBondIssuer
    {
        public Government()
        {
            Taxes = new Taxes();
            Welfare = new Welfare();
            Bonds = new Bonds(this);
            Finances = new GovernmentFinances();
            LaborLaws = new LaborLaws();
        }

        protected override string DefaultName => "Government";
        public new Taxes Taxes { get; }
        public LaborLaws LaborLaws { get; set; }
        public Welfare Welfare { get; }
        public Bonds Bonds { get; }
        public GovernmentFinances Finances { get; }

        public override void FirstTick()
        {
            if (Cash > 0)
                DepositCash(Cash);
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
