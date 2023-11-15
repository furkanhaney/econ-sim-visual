using EconSimVisual.Managers;
using System;

namespace EconSimVisual.Simulation.Agents
{
    [Serializable]
    /// <summary>
    ///     Businesses that buy consumer goods from manufacturers in bulk and sell them to consumers.
    /// </summary>
    internal sealed class Grocer : Vendor
    {
        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "Grocer " + id;

        public Grocer() : base(ConsumerGoods)
        {

        }

    }
}