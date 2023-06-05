using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace RetroBox.Manager.ViewCore
{
    public static class Dialogs
    {
        public static async Task<ButtonResult> ShowMessageBox(string message, string title,
            Icon icon = Icon.Info, ButtonEnum buttons = ButtonEnum.OkCancel, Window? parent = null)
        {
            var pos = parent == null
                ? WindowStartupLocation.CenterScreen
                : WindowStartupLocation.CenterOwner;
            var opts = new MessageBoxStandardParams
            {
                ButtonDefinitions = buttons,
                ContentTitle = title,
                ContentMessage = message,
                Icon = icon,
                WindowIcon = parent?.Icon,
                WindowStartupLocation = pos,
                Topmost = true
            };
            var window = MessageBoxManager.GetMessageBoxStandardWindow(opts);
            FixInnerWindow(window);
            var res = parent == null
                ? await window.Show()
                : await window.ShowDialog(parent);
            return res;
        }

        private static void FixInnerWindow(IMsBoxWindow<ButtonResult> owner)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var inner = owner.GetType().GetField("_window", flags)!;
            var window = (Window)inner.GetValue(owner)!;

            void OnWindowOnOpened(object? o, EventArgs e)
            {
                var sender = (Window)o!;
                FixWindow.RepeatStart(sender);
                sender.Opened -= OnWindowOnOpened;
            }

            window.Opened += OnWindowOnOpened;
        }
    }
}