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
        private PolityTrade Trade => SimulationScreen.Polity.Trade;

        public MarketsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridBankAccounts.SetData(SimulationScreen.Polity.Agents.Banks.SelectMany(o => o.Deposits.Accounts.Values));
            if (Trade.TradeLogs.Count > 0)
                GridCommodities.SetData(Trade.TradeLogs.Last().Summaries.Values);
            GridNewBonds.SetData(Trade.BondExchange.PrimaryMarket);
            GridOldBonds.SetData(Trade.BondExchange.SecondaryMarket);
            GridNewStocks.SetData(Trade.StockExchange.PrimaryMarket);
            GridOldStocks.SetData(Trade.StockExchange.SecondaryMarket);
        }

        public void Initialize()
        {

        }
    }
}
