using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Managers;
using EconSimVisual.Simulation.Securities;

namespace EconSimVisual.Simulation.Agents
{
    using System.Linq;

    using Extensions;
    using Base;
    using Helpers;

    /// <summary>
    ///     A profit-maximing agent
    /// </summary>
    internal abstract class Business : Agent, IBondIssuer
    {
        protected Business()
        {
            Labor = new Labor(this);
            Owners = new Owners(this);
            Ratios = new FinancialRatios(this);
            Bonds = new Bonds(this);
        }

        public FinancialRatios Ratios { get; }
        public BusinessManager Manager { get; set; }
        public Labor Labor { get; }
        public Owners Owners { get; }
        public double BankBalance => BankAccounts.Sum(o => o.Balance);
        public abstract double Revenues { get; }
        public virtual double Expenses => Labor.LastWagesPaid;
        public virtual double Profits => Revenues - Expenses;
        public double Land
        {
            get => Goods[Good.Land];
            set => Goods[Good.Land] = value;
        }

        public override void FirstTick()
        {
            if (Bonds.Exchange == null)
                Bonds.Exchange = Town.Trade.BondExchange;
            Manager.Manage();
            Labor.PayWages();
        }

        public override void LastTick()
        {
            if (IsPeriodStart)
            {
                PayCorporateTax();
                Owners.PayDividends();
                ResetStats();
            }
            base.LastTick();
        }

        protected virtual void ResetStats()
        {
            Labor.ResetStats();
        }

        private void PayCorporateTax()
        {
            if (Profits <= 0) return;
            var corporateTax = Taxes.GetAmount(Profits, TaxType.Corporate);
            if (corporateTax == 0)
                return;

            if (CanPay(corporateTax))
                Taxes.Pay(this, corporateTax, TaxType.Corporate);
            else
                Log(this + " could not pay corporate taxes.", LogType.NonPayment);
        }

        public Bonds Bonds { get; }
    }
}