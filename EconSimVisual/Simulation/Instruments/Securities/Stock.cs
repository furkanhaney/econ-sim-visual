using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    internal class Stock : Security
    {
        public double Dividends => ((Business)Issuer).Owners.Dividends;

        public double TotalDividends => Count * Dividends;

        public override double Value => Count * ((Business)Issuer).BalanceSheet.TotalEquity /
                                        ((Business)Issuer).Owners.OutstandingShares;

        public override Security Clone()
        {
            return new Stock()
            {
                Issuer = Issuer
            };
        }

        public void PayDividends()
        {
            if (Issuer.CanPay(TotalDividends))
            {
                var tax = Taxes.GetAmount(TotalDividends, TaxType.Dividend);
                if (tax > 0)
                    Taxes.Pay(Issuer, tax, TaxType.Dividend);
                Issuer.Pay(Owner, TotalDividends - tax);
            }
            else
                Log(Issuer + " could not make dividend payments to " + Owner, LogType.NonPayment);
        }
    }
}
