using System;
using System.Diagnostics;
using System.Windows;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Government;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;
using System.Linq;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    [Serializable]
    internal class SecurityPurchase : Entity
    {
        public Security Security { get; set; }
        public Agent NewOwner { get; set; }
        public int Count { get; set; }
        public double TotalPrice => Count * Security.UnitPrice;

        public void Execute()
        {
            if (Count > Security.Count)
                MessageBox.Show(Count + " " + Security.Count);
            if (Security is Bond)
                Log(NewOwner + " bought " + Count + " bonds of " + ((Bond)Security).FaceValue.FormatMoney() + " face value for "
                    + TotalPrice.FormatMoney() + " from " + (Security.IsIssued ? Security.Owner : Security.Issuer) + ".", LogType.Securities);
            else if (Security is Stock)
                Log(NewOwner + " bought " + Count + " shares of " + Security.Issuer + " at "
                    + Security.UnitPrice.FormatMoney() + " from " + (Security.IsIssued ? Security.Owner : Security.Issuer) + ".", LogType.Securities);
            if (Security.Count == Count)
                TransferAsWhole();
            else if (Security.Count > Count)
                TransferInPart();

        }

        private void TransferAsWhole()
        {
            HandlePayment();
            if (Security is Stock stock && NewOwner.OwnedSecurities.Any(o => o.Issuer == stock.Issuer && o != stock))
            {
                stock.Count -= Count;
                var same = NewOwner.OwnedSecurities.First(o => o.Issuer == stock.Issuer && o != stock) as Stock;
                same.Count += Count;
                (stock.Issuer as Business).Owners.IssuedStocks.Remove(stock);
                stock.Owner = null;
            }
            else
            {
                Security.Owner = NewOwner;
                if (!Security.IsIssued)
                    Issue(Security);
                else
                    HandleTaxes();
            }
            RemoveFromExchanges();
        }

        private void RemoveFromExchanges()
        {
            if (Security is Bond b)
                Town.Trade.BondExchange.All.Remove(b);
            else if (Security is Stock s)
                Town.Trade.StockExchange.All.Remove(s);
        }

        private void TransferInPart()
        {
            HandlePayment();
            if (Security is Stock stock && NewOwner.OwnedSecurities.Any(o => o.Issuer == stock.Issuer && o != stock))
            {
                Security.Count -= Count;
                Security.OnSaleCount -= Count;
                var same = NewOwner.OwnedSecurities.First(o => o.Issuer == stock.Issuer && o != stock) as Stock;
                same.Count += Count;
                same.BoughtFor += 0;
                HandleTaxes();
            }
            else
            {
                Security.Count -= Count;
                Security.OnSaleCount -= Count;
                var clone = Security.Clone();
                clone.Owner = NewOwner;
                clone.BoughtFor = Security.UnitPrice;
                clone.Count = Count;
                
                if (!Security.IsIssued)
                    Issue(clone);
                else if (clone is Stock s)
                    (Security.Issuer as Business).Owners.IssuedStocks.Add(s);
            }
            if (Security.OnSaleCount == 0)
                RemoveFromExchanges();
        }

        private void CombineIfPossible(Stock stock)
        {
            return;
            if (NewOwner.OwnedSecurities.Any(o => o.Issuer == stock.Issuer && o != stock))
            {
                var same = NewOwner.OwnedSecurities.First(o => o.Issuer == stock.Issuer);
                same.Count += stock.Count;
                stock.Count = 0;
                stock.Owner.OwnedAssets.Remove(stock);
                (stock.Issuer as Business).Owners.IssuedStocks.Remove(stock);
            }
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
            var tax = Math.Max(0, Taxes.GetAmount(capitalGains, TaxType.CapitalGains));
            if (tax != 0)
                Taxes.Pay(Security.Owner, tax, TaxType.CapitalGains);
        }

        private static void Issue(Security security)
        {
            security.IsIssued = true;
            if (security is Bond bond)
            {
                var issuer = security.Issuer as IBondIssuer;
                issuer.Bonds.Issued.Add(bond);

            }
            else if (security is Stock stock)
            {
                var issuer = stock.Issuer as Business;
                issuer.Owners.IssuedStocks.Add(stock);
            }


        }
    }
}
