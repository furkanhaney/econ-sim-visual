namespace EconSimVisual.Simulation.Agents
{
    using Managers;

    /// <summary>
    ///     Businesses that buy consumer goods from manufacturers in bulk and sell them to consumers.
    /// </summary>
    internal sealed class Grocer : Vendor
    {
        private static int count = 1;
        protected override string DefaultName => "Grocer" + count++;

        public Grocer() : base(ConsumerGoods)
        {
            Manager = new GrocerManager(this);
        }

    }
}