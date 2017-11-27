using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for MarketsPanel.xaml
    /// </summary>
    public partial class MarketsPanel : IPanel
    {
        public MarketsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridBankAccounts.SetData(SimulationScreen.Town.Agents.Banks.SelectMany(o => o.Accounts.Values));
            GridCommodities.SetData(SimulationScreen.Town.Trade.TradeLogs.Last());
            GridNewBonds.SetData(SimulationScreen.Town.Trade.BondExchange.PrimaryBonds);
            GridOldBonds.SetData(SimulationScreen.Town.Trade.BondExchange.SecondaryBonds);
        }

        public void Initialize()
        {

        }
    }
}
