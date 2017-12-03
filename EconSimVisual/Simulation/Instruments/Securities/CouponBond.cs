using EconSimVisual.Extensions;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    internal class CouponBond : Bond
    {
        public int PaymentFrequency { get; set; }
        public double CouponRate { get; set; }

        public void PayInterest()
        {
            var payment = FaceValue * Count * CouponRate;
            if (Issuer.CanPay(payment))
                Issuer.Pay(Owner, payment);
            else
                Log(Issuer + " has defaulted on the coupon of a bond!", LogType.NonPayment);
        }
    }
}
