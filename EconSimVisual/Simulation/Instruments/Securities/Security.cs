using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Polities;
using System;

namespace EconSimVisual.Simulation.Instruments.Securities
{
    [Serializable]
    internal abstract class Security : Entity, IAsset
    {
        public Agent Issuer { get; set; }
        private Agent owner;
        public Agent Owner
        {
            get => owner;
            set
            {
                owner?.OwnedAssets.Remove(this);
                owner = value;
                owner?.OwnedAssets.Add(this);
            }
        }

        public override Town Town
        {
            get => Owner.Town;
            set
            {
                throw new System.Exception();
            }
        }
        public bool IsOnSale { get; set; }
        public int OnSaleCount { get; set; }
        public abstract double Value { get; }
        public double UnitPrice { get; set; }

        public bool IsIssued { get; set; }
        public double BoughtFor { get; set; }
        public int Count { get; set; }

        public abstract Security Clone();
    }
}
