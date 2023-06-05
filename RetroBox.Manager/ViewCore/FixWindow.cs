using System;
using System.Reflection;
using Avalonia.Controls;

namespace RetroBox.Manager.ViewCore
{
    public abstract class FixWindow : Window
    {
        private static readonly MethodInfo StartMeth = typeof(Window).GetMethod(
            "SetWindowStartupLocation", BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            
            RepeatStart(this);
        }

        internal static void RepeatStart(Window window)
        {
            var impl = window.Owner?.PlatformImpl;
            StartMeth.Invoke(window, new object?[] { impl });
        }
    }
}