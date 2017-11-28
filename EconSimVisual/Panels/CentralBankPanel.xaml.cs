using System.Collections.Generic;
using System.Linq;
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
        private PolityAgents Agents => SimulationScreen.Town.Agents;

        public CentralBankPanel()
        {
            InitializeComponent();
        }

        private static CentralBank CentralBank => SimulationScreen.Town.Agents.CentralBank;

        public void Update()
        {
            GridBankReserves.SetData(CentralBank.Accounts.Values);
            LblCash.Content = "Cash: " + SimulationScreen.Town.Economy.TotalCash.FormatMoney();
            LblDeposits.Content = "Deposits: " + SimulationScreen.Town.Economy.TotalDeposits.FormatMoney();
            LblMoney.Content = "Money: " + SimulationScreen.Town.Economy.MoneySupply.FormatMoney();

            UpdateCharts();
        }

        private void UpdateCharts()
        {
            PieChartCash.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.Businesses.Sum(o => o.Cash)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.Cash)},
                new PieChartPoint{Name="Government", Value=Agents.Government.Cash},
                new PieChartPoint{Name="Central Bank", Value = Agents.CentralBank.Cash}
            });
            PieChartDeposits.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.Businesses.Sum(o => o.Deposits)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.Deposits)},
                new PieChartPoint{Name="Government", Value=Agents.Government.Deposits},
                new PieChartPoint{Name="Central Bank", Value = Agents.CentralBank.Deposits}
            });
            PieChartMoney.Update(new List<PieChartPoint>()
            {
                new PieChartPoint{Name="Businesses", Value=Agents.Businesses.Sum(o => o.Money)},
                new PieChartPoint{Name="Citizens", Value=Agents.Population.Sum(o => o.Money)},
                new PieChartPoint{Name="Government", Value=Agents.Government.Money},
                new PieChartPoint{Name="Central Bank", Value = Agents.CentralBank.Money}
            });
        }

        public void Initialize()
        {

        }
    }
}
