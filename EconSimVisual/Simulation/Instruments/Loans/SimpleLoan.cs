using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Instruments.Loans
{
    internal class SimpleLoan : Loan
    {
        public Agent Borrower { get; set; }
        public double Principal { get; set; }
        public double InterestRate { get; set; }
        public int MaturityDays { get; set; }
        public int TotalLength { get; set; }
        public override double Value => Payment;
        public double Payment => Principal * Finance.ConvertRate(InterestRate, Finance.AnnualToDaily * TotalLength);

        public void Mature()
        {
            MakePayment();
            SelfDestruct();
        }

        private void MakePayment()
        {
            var payment = Principal * Finance.ConvertRate(InterestRate, Finance.AnnualToDaily * TotalLength);
            if (Borrower.CanPay(payment))
                Borrower.Pay(Owner, payment);
            else
                Town.TownLogger.Log(Borrower + " has defaulted on the principal of a bond!", LogType.NonPayment);
        }

        private void SelfDestruct()
        {
            Owner.OwnedAssets.Remove(this);
            //(Owner as IBondIssuer).Bonds.Issued.Remove(this);
        }
    }
}
