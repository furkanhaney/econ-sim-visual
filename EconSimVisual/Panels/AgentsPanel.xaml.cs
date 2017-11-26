using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Town;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for AgentsPanel.xaml
    /// </summary>
    public partial class AgentsPanel : IPanel
    {
        public AgentsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridCitizens.SetData(Town.Current.Agents.Population);
            GridManufacturers.SetData(Town.Current.Agents.Manufacturers);
            GridGrocers.SetData(Town.Current.Agents.Grocers);
            GridPrivateBanks.SetData(Town.Current.Agents.Banks);
        }

        public void Initialize()
        {
        }
    }
}
