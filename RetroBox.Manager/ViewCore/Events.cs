using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

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

        public static Task<T> Invoke<T>(Func<T> action)
        {
            var ui = Dispatcher.UIThread;
            if (ui.CheckAccess())
            {
                var raw = action();
                return Task.FromResult(raw);
            }
            return ui.InvokeAsync(action);
        }
    }
}