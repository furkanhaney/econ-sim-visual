using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using EconSimVisual.Panels;
using EconSimVisual.Simulation.Polities;
using Xceed.Wpf.AvalonDock.Layout;

namespace EconSimVisual
{
    internal class SimRunner
    {
        public World World { get; }
        public SimulationScreen Gui { get; }
        public bool UpdateContinuously { get; set; } = false;
        private int updateDays;
        private bool stop;

        public SimRunner(World world, SimulationScreen gui)
        {
            World = world;
            Gui = gui;
        }

        public void Run(int days)
        {
            stop = false;
            updateDays = days;
            var thread = new Thread(Process) { IsBackground = true };
            thread.Start();
        }

        public void Stop()
        {
            stop = true;
        }

        private void Process()
        {
            while (!stop && updateDays > 0)
            {
                World.Tick();
                if (UpdateContinuously)
                    UpdateGui();
                updateDays--;
            }
            if (!UpdateContinuously)
                UpdateGui();
            FinalizeGui();
        }

        private void UpdateGui()
        {
            Gui.Dispatcher.Invoke(() =>
            {
                Gui.Update();
            });
        }

        private void FinalizeGui()
        {
            Gui.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            });
        }
    }
}
