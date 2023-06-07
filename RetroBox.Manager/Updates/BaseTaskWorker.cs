using System.Collections.Generic;
using System;
using System.ComponentModel;
using RetroBox.Manager.Models;
using RetroBox.Manager.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace RetroBox.Manager.Updates
{
    internal abstract class BaseTaskWorker<T> : BackgroundWorker
        where T : IProgTask
    {
        protected double _lastPercent;

        protected void DoBaseWork(object? sender, DoWorkEventArgs e)
        {
            var (model, list, token) = (ValueTuple<ProgressViewModel, List<T>, CancellationToken>)
                e.Argument!;
            var all = list.Count;
            var count = 0;
            _lastPercent = 0d;

            foreach (var task in list)
            {
                var artName = ExtractName(task);

                model.CurrentTitle = $"{ActionWhile} '{artName}'...";
                model.CurrentValue = 0;
                UpdateAll(model, count, all);
                model.PaperVis = true;
                model.PaperCol = 0;

                var tmp = DoYourWork(model, task, token);
                var dlFile = tmp.GetAwaiter().GetResult();
                task.Result[0] = dlFile;
                count++;

                model.CurrentTitle = $"{ActionDone} '{artName}'!";
                model.CurrentValue = 100;
                UpdateAll(model, count, all);
                model.PaperVis = false;
                model.PaperCol = 4;

                _lastPercent = 0;
            }
        }

        protected abstract Task<string> DoYourWork(ProgressViewModel model, T task, CancellationToken token);

        protected abstract string ExtractName(T task);

        private static void UpdateAll(ProgressViewModel model, int count, int all)
        {
            model.AllValue = (int)(count / (all * 1d) * 100);
            model.AllTitle = $"{count} of {all} tasks finished";
        }

        protected string? ActionDone { get; set; }
        protected string? ActionWhile { get; set; }
    }
}