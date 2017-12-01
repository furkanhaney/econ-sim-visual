using System.Diagnostics;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Securities
{
    internal class SecurityTransfer : Entity
    {
        public Security Security { get; set; }
        public Agent NewOwner { get; set; }
        public int Count { get; set; }
        public double TotalPrice => Count * Security.UnitPrice;

        public void Execute()
        {
            Debug.Assert(Count <= Security.Count);
            Log(NewOwner + " bought " + Count + " bonds of " + ((Bond)Security).FaceValue.FormatMoney() + " face value for "
                + TotalPrice.FormatMoney() + " from " + (Security.IsIssued ? Security.Owner : Security.Issuer) + ".", LogType.Securities);

            if (Security.Count == Count)
                TransferAsWhole();
            else if (Security.Count > Count)
                TransferInPart();
        }
        private void TransferAsWhole()
        {
            HandlePayment();

            Security.Owner = NewOwner;
            Town.Trade.BondExchange.AllBonds.Remove((Bond)Security);

            if (!Security.IsIssued)
                Issue(Security);
            else
                HandleTaxes();
        }

        private void TransferInPart()
        {
            HandlePayment();

            Security.Count -= Count;
            var clone = Security.Clone();
            clone.Owner = NewOwner;
            clone.BoughtFor = Security.UnitPrice;
            clone.Count = Count;

            if (!Security.IsIssued)
                Issue(clone);
            else
                HandleTaxes();
        }

        private void HandlePayment()
        {
            var totalPrice = Security.UnitPrice * Count;
            NewOwner.Pay(Security.IsIssued ? Security.Owner : Security.Issuer, totalPrice);
            Security.BoughtFor = Security.UnitPrice;
        }

        private void HandleTaxes()
        {
            if (Taxes.Rates[TaxType.CapitalGains] == 0)
                return;
            var capitalGains = Count * (Security.UnitPrice - Security.BoughtFor);
            var tax = Taxes.GetAmount(capitalGains, TaxType.CapitalGains);
            Taxes.Pay(Security.Owner, tax, TaxType.CapitalGains);
        }

        // TODO: Needs more work
        private static void Issue(Security security)
        {
            security.IsIssued = true;
            ((Government.Government)security.Issuer).Bonds.Issued.Add((Bond)security);
        }
    }
}
