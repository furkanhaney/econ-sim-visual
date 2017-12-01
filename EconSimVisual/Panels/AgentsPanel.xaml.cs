using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
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
            GridBusinesses.SetData(Agents.Businesses);
            GridManufacturers.SetData(Agents.Manufacturers);
            GridGrocers.SetData(Agents.Grocers);
            GridPrivateBanks.SetData(Agents.Banks);
            BalanceSheetAgent.Update();
            ComboBoxAgents.SetData(Agents.All);
        }

        public void Initialize()
        {
        }

        private void ComboBoxAgents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            BalanceSheetAgent.Sheet = (Simulation.Accounting.BalanceSheet)((Agent)ComboBoxAgents.SelectedItem).BalanceSheet;
            BalanceSheetAgent.Update();
        }
    }
}
