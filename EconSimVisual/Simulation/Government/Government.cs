using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Government
{
    internal class Government : Agent
    {
        public Government()
        {
            TargetCash = 200000;
            Taxes = new Taxes();
            Welfare = new Welfare();
            Debt = new GovernmentDebt(this);
            Finances = new GovernmentFinances();
        }

        protected override string DefaultName => "Government";
        public new Taxes Taxes { get; }
        public Welfare Welfare { get; }
        public GovernmentDebt Debt { get; }
        public GovernmentFinances Finances { get; }

        public override void FirstTick()
        {

        }

        public override void LastTick()
        {
            Taxes.LastTick();
            Welfare.Tick();
        }
    }
}
