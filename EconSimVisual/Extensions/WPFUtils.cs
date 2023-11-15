using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace EconSimVisual.Extensions
{
    internal static class WPFUtils
    {
        public static void SetData(this ComboBox cmb, IEnumerable data)
        {
            var item = cmb.SelectedItem;
            cmb.ItemsSource = data;
            if (cmb.ItemsSource.Cast<object>().Contains(item))
                cmb.SelectedValue = item;
            else
                cmb.SelectedIndex = 0;
        }
        public static void SetData(this DataGrid grid, IEnumerable data)
        {
            grid.ItemsSource = null;
            grid.ItemsSource = data;
        }
        public static void SetData(this DataGrid grid, IDictionary dict)
        {

        }

        public static void Update<T>(this ComboBox cmb, IEnumerable<T> items)
        {
            cmb.ItemsSource = null;
            cmb.ItemsSource = items;
            if (items.Any())
                cmb.SelectedIndex = 0;
        }

        public static void UpdateColored(this Label lbl, double value)
        {
            lbl.Content = value.FormatMoney();
            if (value > 0)
                lbl.Foreground = Brushes.Green;
            else if (value < 0)
                lbl.Foreground = Brushes.Red;
            else
                lbl.Foreground = Brushes.Black;
        }
    }
}
