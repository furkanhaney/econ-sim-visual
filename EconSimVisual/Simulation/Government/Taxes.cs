using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Government
{
    internal enum TaxType { Corporate, Income, Dividend, Sales, Property, CapitalGains, Estate }

    internal class Taxes : Entity
    {
        public Taxes()
        {
            Rates = CollectionsExtensions.InitializeDictionary<TaxType>();
            CurrentRevenues = CollectionsExtensions.InitializeDictionary<TaxType>();

            Summaries = new List<TaxSummary>();
        }

        public List<TaxSummary> Summaries { get; }
        public Dictionary<TaxType, double> Rates { get; }
        public Dictionary<TaxType, double> CurrentRevenues { get; }
        public Dictionary<TaxType, double> LastRevenues
        {
            get
            {
                if (Summaries.Count == 0)
                    return CollectionsExtensions.InitializeDictionary<TaxType>();
                return Summaries.Last().Revenues;
            }
        }

        public double CurrentRevenue => CurrentRevenues.Values.Sum();
        public double LastRevenue => LastRevenues.Values.Sum();

        public void LastTick()
        {
            if (IsPeriodStart)
                ResetRevenues();
        }

        public void ResetRevenues()
        {
            Summaries.Add(new TaxSummary
            {
                Day = Entity.Day,
                Rates = Rates,
                Revenues = CurrentRevenues.Clone()
            });
            CurrentRevenues.Reset();
        }

        public double GetAmount(double totalAmount, TaxType taxType)
        {
            return totalAmount * Rates[taxType];
        }

        public void Pay(Agent payer, double amount, TaxType taxType)
        {
            Debug.Assert(payer.CanPay(amount));

            payer.Pay(Government, amount);
            CurrentRevenues[taxType] += amount;
        }
    }
}