using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.API;
using RetroBox.API.Update;
using RetroBox.Fabric;
using RetroBox.Manager.Models;
using RetroBox.Manager.Updates;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;
using RetroBox.Update;
using IOPath = System.IO.Path;

namespace RetroBox.Manager.Views
{
    public partial class EmuUpdateWindow : FixWindow
    {
        private readonly IReleaseFetcher _github;

        public EmuUpdateWindow()
        {
            InitializeComponent();
            _github = new GithubFetcher();
        }

        private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Cancel);
        }

        private async void BtnDownload_OnClick(object? sender, RoutedEventArgs e)
        {
            var emus = lvEmus.GetChecked<Release>().ToArray();
            var roms = lvRoms.GetChecked<Release>().ToArray();

            var configDir = Platforms.Sys.GetDefaultConfigPath();
            var emuDir = IOPath.Combine(configDir, "emu");
            var romDir = IOPath.Combine(configDir, "rom");

            var list = new List<DownloadTask>();
            foreach (var item in emus)
                list.Add(new(item, emuDir, new string[1]));
            foreach (var item in roms)
                list.Add(new(item, romDir, new string[1]));

            if (list.Count >= 1)
            {
                var model = new ProgressViewModel
                {
                    CurrentTitle = "...", CurrentValue = 0,
                    AllTitle = "...", AllValue = 0,
                    PaperCol = 0, PaperVis = false
                };
                var progress = new ProgressWindow { DataContext = model };
                var token = progress.CancellationToken;

                using var dlWorker = new DownloadWorker();
                model.TaskType = ProgTaskType.Download;
                dlWorker.RunWorkerCompleted += (_, ds) =>
                {
                    if (ds.Cancelled)
                        return;
                    using var exWorker = new ExtractWorker();
                    model.TaskType = ProgTaskType.Extract;
                    exWorker.RunWorkerCompleted += (_, es) =>
                    {
                        if (es.Cancelled)
                            return;
                        progress.Close(ButtonResult.Ok);
                    };
                    var exList = ExtractWorker.Convert(list);
                    exWorker.RunWorkerAsync((model, exList, token));
                };
                dlWorker.RunWorkerAsync((model, list, token));

                await progress.ShowDialogFor(this);
            }

            Close(ButtonResult.Ok);
        }

        private async void TopLevel_OnOpened(object? sender, EventArgs e)
        {
            var allEmus = new List<Release>();
            await foreach (var release in _github.FetchEmuReleases())
                allEmus.Add(release);
            var allRoms = new List<Release>();
            await foreach (var release in _github.FetchRomReleases())
                allRoms.Add(release);
            allEmus = allEmus.FindMatch();
            allRoms = allRoms.FindMatch(ignoreSrc: false);

            var model = (EmuUpdateViewModel)DataContext!;
            foreach (var release in allEmus)
                model.Emus.Add(release);
            foreach (var release in allRoms)
                model.Roms.Add(release);
        }
    }
}