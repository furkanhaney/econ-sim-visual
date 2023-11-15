using EconSimVisual.Initializers;
using EconSimVisual.Simulation.Polities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EconSimVisual
{
    /// <summary>
    /// Interaction logic for Benchmark.xaml
    /// </summary>
    public partial class Benchmark : Window
    {
        public Benchmark()
        {
            InitializeComponent();
            lblTotalTime.Content = TimeSpan.Zero.ToString(@"mm\:ss\.fff");
            lblAverageTime.Content = TimeSpan.Zero.ToString(@"mm\:ss\.fff");
        }

        private World World { get; set; }
        private int PastDays { get; set; }
        private int Days { get; set; }
        private int Scale { get; set; }
        private bool IsRunning { get; set; }
        private bool SimIsRunning { get; set; }
        private DateTime Start { get; set; }
        private DateTime Last { get; set; }
        private DateTime End { get; set; }
        private Thread SimThread { get; set; }
        private Thread GuiThread { get; set; }
        private IWorldInitializer Initializer { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning)
            {
                SimThread.Abort();
                btnRun.Content = "Run";
                IsRunning = false;
                UpdateUI();
            }
            else
            {
                lblDays.Content = "Initializing...";
                Days = (int)upDownDays.Value;
                Scale = (int)upDownScale.Value;
                Initializer = new MediumWorldInitializer() { Scale = Scale };
                SimThread = new Thread(RunSim);
                SimThread.Start();
                (new Thread(RunGui)).Start();
                btnRun.Content = "Stop";
            }
        }

        private void RunSim()
        {
            Start = DateTime.UtcNow;
            PastDays = 0;
            IsRunning = true;
            World = new World(Initializer);
            SimIsRunning = true;
            Start = DateTime.UtcNow;
            for (int i = 0; i < Days; i++)
            {
                World.Tick();
                PastDays++;
                Last = DateTime.UtcNow;
            }
            SimIsRunning = false;
            IsRunning = false;
            End = DateTime.UtcNow;
            Dispatcher.Invoke(() => { FinalizeUI(); });
        }

        private void RunGui()
        {
            while (SimThread.IsAlive)
            {
                Thread.Sleep(125);
                Dispatcher.Invoke(() => { UpdateUI(); });
            }
        }

        private void UpdateUI()
        {
            var pastTime = IsRunning ? DateTime.UtcNow - Start : End - Start;
            var lastTime = IsRunning ? Last - Start : End - Start;
            var avgTime = PastDays > 0 ? TimeSpan.FromTicks(lastTime.Ticks / PastDays) : TimeSpan.Zero;

            prgDays.Value = 100 * PastDays / Days;
            lblDays.Content = PastDays + " / " + Days;
            lblTotalTime.Content = pastTime.ToString(@"mm\:ss\.fff");
            lblAverageTime.Content = avgTime.ToString(@"mm\:ss\.fff");
            if (IsRunning && !SimIsRunning)
                lblDays.Content = "Initializing...";
        }

        private void FinalizeUI()
        {
            btnRun.Content = "Run";
        }
    }
}