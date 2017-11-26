namespace EconSimVisual
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Hide();
            (new SimulationScreen()).Show();
        }
    }
}
