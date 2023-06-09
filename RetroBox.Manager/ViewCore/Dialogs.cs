using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static async Task<ButtonResult?> ShowDialogFor(this Window window, Window parent)
        {
            window.Icon = parent.Icon;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Topmost = true;
            window.Tag = parent;
            var res = await window.ShowDialog<ButtonResult?>(parent);
            return res;
        }

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

        public static async Task<string?> AskToOpenFile(Window parent, string title, string ext, string kind)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false, Title = title,
                Filters = new List<FileDialogFilter>
                {
                    new() { Name = $"{kind} (.{ext})", Extensions = new List<string> { ext } }
                }
            };
            var fileNames = await dialog.ShowAsync(parent);
            if (fileNames?.Length >= 1)
            {
                var fileName = fileNames.First();
                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                    return fileName;
            }
            return null;
        }

        public static async Task<string?> AskToOpenFolder(Window parent, string title, string? initialDir)
        {
            var dialog = new OpenFolderDialog
            {
                Title = title, Directory = initialDir
            };
            var folder = await dialog.ShowAsync(parent);
            if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
                return folder;
            return null;
        }

        public static Window? GetParent<T>(this Window window)
            where T : Window
        {
            var owner = window.Owner as Window ?? window.Parent as Window ?? window.Tag as Window;
            var conv = owner as T;
            return conv;
        }
    }
}