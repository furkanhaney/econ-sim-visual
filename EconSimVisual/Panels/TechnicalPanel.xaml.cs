using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Securities;
using EconSimVisual.Simulation.Town;
using Microsoft.VisualBasic.Devices;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for TechnicalPanel.xaml
    /// </summary>
    public partial class TechnicalPanel : IPanel
    {
        public DateTime StartTime { get; set; }

        public TechnicalPanel()
        {
            InitializeComponent();
        }

        public void Update()
        {
            LblUpdateTime.Content = (DateTime.UtcNow - StartTime).TotalMilliseconds.ToString("0.00") + " ms";
            LblTotalEntities.Content = Entity.TotalCount.ToString("###,##0");

            LblRamUsage.Content = GetUsedMemory().FormatFileSize() + " / " +
                                  (GetAvailableMemory() + GetUsedMemory()).FormatFileSize();
        }

        public void Initialize()
        {
        }

        private static double GetAvailableMemory()
        {
            return double.Parse(new ComputerInfo().AvailablePhysicalMemory.ToString());
        }

        private static double GetUsedMemory()
        {
            using (var proc = Process.GetCurrentProcess())
                return proc.PrivateMemorySize64;
        }

        private void Test_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var asset in Town.Current.Agents.Banks.SelectMany(o => o.OwnedAssets))
            {
                var bond = (Bond)asset;
                MessageBox.Show(bond.Owner + " " + bond.FaceValue + " " + bond.Count);
            }
        }
    }
}
