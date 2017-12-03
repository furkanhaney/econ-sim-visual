using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Helpers;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    internal class Bond : Security
    {
        public double FaceValue { get; set; }
        public int MaturityDays { get; set; }
        public double Yield => Finance.GetYield(FaceValue, UnitPrice, MaturityDays);
        public override double Value => FaceValue * Count; // TODO: Use market price of the bond

        public void Mature()
        {
            MakePayment();
            SelfDestruct();
        }

        public override Security Clone()
        {
            return new Bond
            {
                Issuer = Issuer,
                Count = Count,
                IsIssued = IsIssued,
                BoughtFor = BoughtFor,
                FaceValue = FaceValue,
                UnitPrice = UnitPrice,
                MaturityDays = MaturityDays
            };
        }

        private void MakePayment()
        {
            var payment = FaceValue * Count;
            if (Issuer.CanPay(FaceValue * Count))
                Issuer.Pay(Owner, payment);
            else
                Town.TownLogger.Log(Issuer + " has defaulted on the principal of a bond!", LogType.NonPayment);
        }

        private void SelfDestruct()
        {
            Owner.OwnedAssets.Remove(this);
            (Issuer as IBondIssuer).Bonds.Issued.Remove(this);
        }
    }
}
