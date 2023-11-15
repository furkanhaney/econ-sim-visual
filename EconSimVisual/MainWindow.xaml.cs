using EconSimVisual.Extensions;
using EconSimVisual.Initializers;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hide();
            IWorldInitializer initializer = new MediumWorldInitializer();
            var world = new World(initializer);
            new SimulationScreen(world).Show();
        }

        private void btnLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hide();
            var path = @"C:\Users\Furkan\Documents\EconSim\" + txtSaveName.Text + ".bin";
            var world = Serializer.BinaryDeserialize<World>(path);
            new SimulationScreen(world).Show();
        }

        private void btnBenchmark_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (new Benchmark()).Show();
            Close();
        }
    }
}
