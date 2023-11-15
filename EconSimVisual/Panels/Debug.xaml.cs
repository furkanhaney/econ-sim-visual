using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Securities;
using Microsoft.VisualBasic.Devices;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EconSimVisual.Panels
{
    /// <summary>
    /// Interaction logic for Debug.xaml
    /// </summary>
    public partial class Debug : IPanel
    {
        public static DateTime StartTime { get; set; }

        public Debug()
        {
            InitializeComponent();
        }

        public void Update()
        {
            var agents = SimulationScreen.Polity.Agents;
            var econ = SimulationScreen.Polity.Economy;

            LblTotalCash.Content = agents.All.Sum(o => o.Cash).FormatMoney();
            if (SimulationScreen.Polity.Agents.CentralBank != null)
                LblCentralBankCash.Content = agents.CentralBank.Cash.FormatMoney();
            LblMonetaryBase.Content = econ.MonetaryBase.FormatMoney();
            LblMoneyMultiplier.Content = econ.MoneyMultiplier.ToString("0.00");


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
            foreach (var asset in SimulationScreen.Polity.Agents.Banks.SelectMany(o => o.OwnedAssets))
            {
                var bond = (Bond)asset;
                MessageBox.Show(bond.Owner + " " + bond.FaceValue + " " + bond.Count);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var path = @"C:\Users\Furkan\Documents\EconSim\" + txtSaveName.Text + ".bin";
            Serializer.BinarySerialize(SimulationScreen.World, path);
        }
    }
}
