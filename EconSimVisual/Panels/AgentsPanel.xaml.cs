using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for AgentsPanel.xaml
    /// </summary>
    public partial class AgentsPanel : IPanel
    {
        private static PolityAgents Agents => SimulationScreen.Town.Agents;

        public AgentsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridCitizens.SetData(Agents.Population);
            GridManufacturers.SetData(Agents.Manufacturers);
            GridGrocers.SetData(Agents.Grocers);
            GridPrivateBanks.SetData(Agents.Banks);
        }

        public void Initialize()
        {
        }
    }
}
