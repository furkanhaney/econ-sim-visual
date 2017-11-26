using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Town;

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

        private static CentralBank CentralBank => Town.Current.Agents.CentralBank;

        public void Update()
        {
            GridBankReserves.SetData(CentralBank.Accounts.Values);
        }

        public void Initialize()
        {

        }
    }
}
