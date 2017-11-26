namespace EconSimVisual.Simulation.Base
{
    internal interface IAsset
    {
        Agent Owner { get; set; }
        double Value { get; }
    }
}
