using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.API;
using RetroBox.API.Update;
using RetroBox.Fabric;
using RetroBox.Manager.Models;
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
            var emuDir = IOPath.Combine(configDir, "emulators");
            var romDir = IOPath.Combine(configDir, "roms");

            var list = new List<DownloadTask>();
            foreach (var item in emus)
                list.Add(new(item, emuDir));
            foreach (var item in roms)
                list.Add(new(item, romDir));

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

                using var worker = new BackgroundWorker();
                worker.RunWorkerCompleted += (_, s) =>
                {
                    if (!s.Cancelled)
                        progress.Close(ButtonResult.Ok);
                };
                worker.DoWork += DoWork;
                worker.RunWorkerAsync((model, list, token));

                await progress.ShowDialogFor(this);
            }

            // TODO Download and extract ?!

            Close(ButtonResult.Ok);
        }

        private void DoWork(object? sender, DoWorkEventArgs e)
        {
            var (model, list, token) = (ValueTuple<ProgressViewModel, List<DownloadTask>, CancellationToken>)
                e.Argument!;
            var all = list.Count;
            var count = 0;
            var lastPercent = 0d;

            void Handler(HttpRequestMessage _, HttpProgressEventArgs a)
            {
                var total = a.TotalBytes ?? 46_999_999;
                var percent = a.BytesTransferred / (total * 1d);
                model.CurrentValue = (int)(percent * 100);
                if (percent - lastPercent >= 0.03)
                {
                    model.PaperCol++;
                    lastPercent = percent;
                }
                if (model.PaperCol >= 5)
                    model.PaperCol = 0;
            }

            foreach (var task in list)
            {
                var relId = task.Release.Id;
                var art = task.Release.Artifacts.First();
                var artName = art.Url.Split('/').Last();
                var artUrl = art.Url;
                var artFile = IOPath.Combine(task.Target, relId, artName);

                model.CurrentTitle = $"Downloading '{artName}'...";
                model.CurrentValue = 0;
                UpdateAll(model, count, all);
                model.PaperVis = true;
                model.PaperCol = 0;

                var tmp = WebLoader.Download(artUrl, artFile, Handler, token);
                tmp.GetAwaiter().GetResult();
                count++;

                model.CurrentTitle = $"Downloaded '{artName}'!";
                model.CurrentValue = 100;
                UpdateAll(model, count, all);
                model.PaperVis = false;
                model.PaperCol = 4;

                lastPercent = 0;
            }
        }

        private static void UpdateAll(ProgressViewModel model, int count, int all)
        {
            model.AllValue = (int)(count / (all * 1d) * 100);
            model.AllTitle = $"{count} of {all} tasks finished";
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