using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    using Simulation.Agents;

    /// <summary>
    /// Interaction logic for CentralBankPanel.xaml
    /// </summary>
    public partial class CentralBankPanel : IPanel
    {
        public CentralBankPanel()
        {
            InitializeComponent();
        }

        private static CentralBank CentralBank => SimulationScreen.Town.Agents.CentralBank;

        public void Update()
        {
            GridBankReserves.SetData(CentralBank.Accounts.Values);
            LblCurrentCash.Content = "Current Cash: " + CentralBank.Cash.FormatMoney();
        }

        public void Initialize()
        {

        }
    }
}
