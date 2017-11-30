using EconSimVisual.Simulation.Government;

namespace EconSimVisual.Simulation.Securities
{
    using Extensions;
    using Agents;

    internal class Stock : Security
    {
        public double Dividends => Count * ((Business)Issuer).Owners.Dividends;

        public override Security Clone()
        {
            return new Stock()
            {
                Issuer = Issuer
            };
        }

        public void PayDividends()
        {
            if (Issuer.CanPay(Dividends))
            {
                var tax = Taxes.GetAmount(Dividends, TaxType.Dividend);
                if (tax > 0)
                    Taxes.Pay(Issuer, tax, TaxType.Dividend);
                Issuer.Pay(Owner, Dividends - tax);
            }
            else
                Log(Issuer + " could not make dividend payments to " + Owner, LogType.NonPayment);
        }
    }
}
