using EconSimVisual.Simulation.Town;

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
            var logs = Town.Current.TownLogger.GetLogs(filters, timeFrame);
            GridLogs.SetData(logs);
            BtnClearLogs.Content = "Clear (" + Town.Current.TownLogger.Count.ToString("###,##0") + ")";
        }

        private void ClearLogs(object sender, RoutedEventArgs e)
        {
            Town.Current.TownLogger.Clear();
            UpdateLogs(null, null);
        }
    }
}
