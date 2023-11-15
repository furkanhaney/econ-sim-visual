using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for AgentsPanel.xaml
    /// </summary>
    public partial class AgentsPanel : IPanel
    {
        private static PolityAgents Agents => SimulationScreen.Polity.Agents;

        public AgentsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridCitizens.SetData(Agents.Population);
            GridBusinesses.SetData(Agents.Businesses);
            GridManufacturers.SetData(Agents.AllManufacturers);
            GridGrocers.SetData(Agents.Grocers);
            GridPrivateBanks.SetData(Agents.Banks);
            BalanceSheetAgent.Update();
            ComboBoxAgents.SetData(Agents.Businesses);
            ComboBoxBusinesses.SetData(Agents.Businesses);
            ComboBoxBusinesses2.SetData(Agents.Businesses);
            IncomeStatementView.Update();
        }

        public void Initialize()
        {
        }

        private void ComboBoxAgents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBoxAgents.SelectedItem != null)
            {
                BalanceSheetAgent.Sheet = (Simulation.Accounting.BalanceSheet)((Agent)ComboBoxAgents.SelectedItem).BalanceSheet;
                BalanceSheetAgent.Update();
            }
        }

        private void ComboBoxBusinesses_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBoxBusinesses.SelectedItem != null)
            {
                var business = (Business)ComboBoxBusinesses.SelectedItem;
                GridStocks.SetData(business.Owners.IssuedStocks);
            }
        }

        private void ComboBoxBusinesses2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IncomeStatementView.Business = (Business)ComboBoxBusinesses2.SelectedItem;
        }
    }
}
