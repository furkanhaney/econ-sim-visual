using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Panels
{
    using System.Windows;

    using Extensions;

    /// <summary>
    /// Interaction logic for LogsPanel.xaml
    /// </summary>
    public partial class LogsPanel : IPanel
    {
        public LogsPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            UpdateLogs(null, null);
        }

        public void Initialize()
        {
            FilterComboBox.ItemsSource =EnumUtils.GetValues<LogType>();
            CmbLogsTimeFrame.SetData(ChartsPanel.TimeFrames);
        }

        private void UpdateLogs(object sender, RoutedEventArgs e)
        {
            var filters = FilterComboBox.SelectedItems;
            var timeFrame = (int)CmbLogsTimeFrame.SelectedItem;
            var logs = SimulationScreen.Polity.TownLogger.GetLogs(filters, timeFrame);
            GridLogs.SetData(logs);
            BtnClearLogs.Content = "Clear (" + SimulationScreen.Polity.TownLogger.Count.ToString("###,##0") + ")";
        }

        private void ClearLogs(object sender, RoutedEventArgs e)
        {
            SimulationScreen.Polity.TownLogger.Clear();
            UpdateLogs(null, null);
        }
    }
}
