﻿namespace EconSimVisual.Simulation.Securities
{
    using Extensions;

    internal class Bond : Security
    {
        public double FaceValue { get; set; }
        public int MaturityDays { get; set; }
        public double Yield => Finance.GetYield(FaceValue, UnitPrice, MaturityDays);
        public override double Value => FaceValue * Count; // TODO: Use market price of the bond

        public void Mature()
        {
            MakePayment();
            Owner.OwnedAssets.Remove(this);
            ((Government.Government)Issuer).Debt.IssuedBonds.Remove(this);
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
    }
}
