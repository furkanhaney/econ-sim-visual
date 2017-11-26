namespace EconSimVisual.Simulation.Securities
{
    using Base;

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
                owner.OwnedAssets.Add(this);
            }
        }

        public virtual double Value { get; }
        public double UnitPrice { get; set; }

        public bool IsIssued { get; set; }
        public double BoughtFor { get; set; }
        public int Count { get; set; }

        public abstract Security Clone();
    }
}
