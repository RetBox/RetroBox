using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using RetroBox.Manager.Models;
using RetroBox.Manager.ViewModels;
using RetroBox.Update;

namespace RetroBox.Manager.Updates
{
    internal sealed class ExtractWorker : BaseTaskWorker<ExtractTask>
    {
        public ExtractWorker()
        {
            ActionWhile = "Extracting";
            ActionDone = "Extracted";
            DoWork += DoBaseWork;
        }

        protected override Task<string> DoYourWork(ProgressViewModel model, ExtractTask task, CancellationToken token)
        {
            var input = task.File;
            var outPath = task.Folder;
            var res = ZipLoader.UnCompress(input, outPath,
                (o, a) => Handler(model, a, (ZipHolder)o), token);
            return res;
        }

        private void Handler(ProgressViewModel model, ProgressEventArgs a, ZipHolder info)
        {
            info.ProcessedBytes += a.Processed;

            var total = info.UncompressedSize;
            var percent = info.ProcessedBytes / (total * 1d);
            model.CurrentValue = (int)(percent * 100);
            if (percent - _lastPercent >= 0.03)
            {
                model.PaperCol++;
                _lastPercent = percent;
            }
            if (model.PaperCol >= 5)
                model.PaperCol = 0;
        }

        protected override string ExtractName(ExtractTask task)
        {
            var name = Path.GetFileName(task.File);
            return name;
        }

        public static List<ExtractTask> Convert(IEnumerable<DownloadTask> tasks)
        {
            var exList = new List<ExtractTask>();
            foreach (var task in tasks)
            {
                var exInput = task.Result[0];
                var exDir = Path.Combine(Path.GetDirectoryName(exInput)!, "e");
                var exItem = new ExtractTask(exInput, exDir, new string[1]);
                exList.Add(exItem);
            }
            return exList;
        }
    }
}