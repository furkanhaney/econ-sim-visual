using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Instruments.Loans
{
    internal abstract class Loan : Entity, IAsset
    {
        private Agent owner;
        public Agent Owner
        {
            get => owner;
            set
            {
                owner?.OwnedAssets.Remove(this);
                owner = value;
                owner.OwnedAssets.Add(this);
            }
        }

        public abstract double Value { get; }
    }
}
