using System.IO;
using System.Linq;
using System.Net.Http.Handlers;
using System.Threading;
using System.Threading.Tasks;
using RetroBox.Manager.Models;
using RetroBox.Manager.ViewModels;
using RetroBox.Update;

namespace RetroBox.Manager.Updates
{
    internal sealed class DownloadWorker : BaseTaskWorker<DownloadTask>
    {
        public DownloadWorker()
        {
            ActionWhile = "Downloading";
            ActionDone = "Downloaded";
            DoWork += DoBaseWork;
        }

        private (string name, string url, string file) ExtractDetails(DownloadTask task)
        {
            var relId = task.Release.Id;
            var art = task.Release.Artifacts.First();
            var artName = art.Url.Split('/').Last();
            var artUrl = art.Url;
            var artFile = Path.Combine(task.Folder, relId, artName);
            return (artName, artUrl, artFile);
        }

        private void Handler(ProgressViewModel model, HttpProgressEventArgs a)
        {
            var total = a.TotalBytes ?? 46_999_999;
            var percent = a.BytesTransferred / (total * 1d);
            model.CurrentValue = (int)(percent * 100);
            if (percent - _lastPercent >= 0.03)
            {
                model.PaperCol++;
                _lastPercent = percent;
            }
            if (model.PaperCol >= 5)
                model.PaperCol = 0;
        }

        protected override Task<string> DoYourWork(ProgressViewModel model, DownloadTask task, CancellationToken token)
        {
            var (_, url, file) = ExtractDetails(task);
            var res = WebLoader.Download(url, file, (_, a) => Handler(model, a), token);
            return res;
        }

        protected override string ExtractName(DownloadTask task)
        {
            return ExtractDetails(task).name;
        }
    }
}