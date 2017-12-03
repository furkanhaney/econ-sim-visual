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
        private TradeManager Trade => SimulationScreen.Town.Trade;

        public MarketsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            GridBankAccounts.SetData(SimulationScreen.Town.Agents.Banks.SelectMany(o => o.Accounts.Values));
            GridCommodities.SetData(Trade.TradeLogs.Last());
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
