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

            var impl = Owner?.PlatformImpl;
            StartMeth.Invoke(this, new object?[] { impl });
        }
    }
}