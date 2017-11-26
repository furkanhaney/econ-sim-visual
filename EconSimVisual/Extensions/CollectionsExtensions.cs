namespace EconSimVisual.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Controls;

    internal static class CollectionsExtensions
    {
        public static void Sort(this DataGrid dataGrid, int columnIndex = 0, bool ascending = true)
        {
            var sortDirection = @ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) { action(item); }
        }
        public static IList<T> Clone<T>(this IList<T> list) => new List<T>(list);
        public static Dictionary<A, B> Clone<A, B>(this IDictionary<A, B> dict) => new Dictionary<A, B>(dict);
        public static Dictionary<T, double> InitializeDictionary<T>(IEnumerable<T> keys = null, double defaultValue = 0)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T has to be an enum!");
            var dictionary = new Dictionary<T, double>();
            foreach (var value in keys?.ToArray() ?? Enum.GetValues(typeof(T)))
                dictionary.Add((T)value, defaultValue);
            return dictionary;
        }
        public static void Reset<TKey>(this IDictionary<TKey, double> dict)
        {
            foreach (var key in dict.Keys.ToList())
                dict[key] = 0;
        }
    }
}
