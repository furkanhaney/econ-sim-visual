using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Town;

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
            GridBankAccounts.SetData(Town.Current.Agents.Banks.SelectMany(o => o.Accounts.Values));
            GridCommodities.SetData(Town.Current.Trade.TradeLogs.Last());
            GridNewBonds.SetData(Town.Current.Trade.BondExchange.PrimaryBonds);
            GridOldBonds.SetData(Town.Current.Trade.BondExchange.SecondaryBonds);
        }

        public void Initialize()
        {

        }
    }
}
