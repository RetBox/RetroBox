using System;
using Avalonia;
using Avalonia.Controls;

namespace RetroBox.Manager.ViewCore
{
    public static class Events
    {
        public static void OnTextChange(this TextBox box, Action<TextBox, string> action)
        {
            var observe = box.GetObservable(TextBox.TextProperty);
            observe.Subscribe(text => action(box, text));
        }
    }
}