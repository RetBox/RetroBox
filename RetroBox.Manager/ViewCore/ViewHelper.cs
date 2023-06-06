using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace RetroBox.Manager.ViewCore
{
    internal static class ViewHelper
    {
        public static IEnumerable<T> GetChecked<T>(this IVisual list)
        {
            return list.GetVisualDescendants().OfType<CheckBox>()
                .Where(c => c.IsChecked == true)
                .Select(c => (T)c.DataContext!);
        }
    }
}