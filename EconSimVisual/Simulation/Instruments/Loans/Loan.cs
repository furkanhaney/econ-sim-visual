using EconSimVisual.Simulation.Base;
using System;

namespace EconSimVisual.Simulation.Instruments.Loans
{
    [Serializable]
    internal abstract class Loan : Entity, IAsset
    {
        public Agent Borrower { get; set; }
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
