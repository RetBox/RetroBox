using System;
using Avalonia;
using Avalonia.Controls;

namespace RetroBox.Manager.ViewCore
{
    public static class Events
    {
        public static IDisposable OnTextChange(this TextBox box, Action<TextBox, string> action)
        {
            var observe = box.GetObservable(TextBox.TextProperty);
            var sub = observe.Subscribe(text => action(box, text));
            return sub;
        }
    }
}