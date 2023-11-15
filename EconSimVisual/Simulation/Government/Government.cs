using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Instruments.Securities;
using EconSimVisual.Simulation.Polities;
using System;

namespace EconSimVisual.Simulation.Government
{
    [Serializable]
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

        public override Town Town
        {
            get => base.Town;
            set
            {
                base.Town = value;
                Taxes.Town = value;
                Finances.Town = value;
                Welfare.Town = value;
            }
        }

        public override Polity Polity
        {
            get => base.Polity;
            set
            {
                base.Polity = value;
                Taxes.Polity = value;
                Finances.Polity = value;
                Welfare.Polity = value;
            }
        }

        public new Taxes Taxes { get; }
        public LaborLaws LaborLaws { get; set; }
        public Welfare Welfare { get; }
        public Bonds Bonds { get; }
        public GovernmentFinances Finances { get; }

        public override void FirstTick()
        {
            if (Cash > 0)
                DepositCash(Cash);
        }
        public override void LastTick()
        {
            Bonds.Tick();
            Taxes.LastTick();
            Welfare.Tick();
            base.LastTick();
        }
    }
}
