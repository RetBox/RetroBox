﻿using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using RetroBox.API;
using RetroBox.API.Update;

namespace RetroBox.Update
{
    public sealed class GithubFetcher : IReleaseFetcher
    {
        private readonly WebCache _cache;
        private readonly string _emuUrl;
        private readonly string _romUrl;
        private readonly string _baseUrl;

        public GithubFetcher(WebCache cache, string baseUrl = "https://github.com")
        {
            _cache = cache;
            _baseUrl = baseUrl;
            _emuUrl = $"{baseUrl}/86Box/86Box/releases";
            _romUrl = $"{baseUrl}/86Box/roms/releases";
        }

        public IAsyncEnumerable<Release> FetchEmuReleases()
        {
            var res = FetchWebReleases(_emuUrl, _baseUrl);
            return res;
        }

        public IAsyncEnumerable<Release> FetchRomReleases()
        {
            var res = FetchWebReleases(_romUrl, _baseUrl);
            return res;
        }

        private async IAsyncEnumerable<Release> FetchWebReleases(string url, string baseUrl)
        {
            var stream = await _cache.GetStreamAsync(url);
            var doc = new HtmlDocument();
            doc.Load(stream);
            var node = doc.DocumentNode.Descendants(1).FirstOrDefault(n => n.HasClass("repository-content"));
            if (node == null)
                yield break;
            foreach (var relNode in node.Descendants(1).Where(n => n.Name == "section"))
            {
                var relName = relNode.Descendants(1)
                    .FirstOrDefault(n => n.HasClass("sr-only"))?.InnerText.Trim();
                if (relName == null)
                    continue;
                var relFooter = relNode.Descendants(1)
                    .FirstOrDefault(n => n.HasClass("Box-footer"));
                if (relFooter == null)
                    continue;
                var relLazy = relFooter.Descendants(1)
                    .FirstOrDefault(n => n.Name == "include-fragment")
                    ?.GetAttributeValue("src", null);
                if (relLazy == null)
                    continue;
                var relId = relLazy.Split('/').Last();
                var lazyStream = await _cache.GetStreamAsync(relLazy);
                var lazyDoc = new HtmlDocument();
                lazyDoc.Load(lazyStream);
                var relLinks = lazyDoc.DocumentNode.Descendants(1)
                    .Where(n => n.Name == "a");
                var arts = new List<Artifact>();
                foreach (var relLink in relLinks)
                {
                    var relLinkName = relLink.Descendants(1)
                        .FirstOrDefault(n => n.Name == "span")?.InnerText.Trim();
                    if (relLinkName == null)
                        continue;
                    var relLinkUrl = baseUrl + relLink.GetAttributeValue("href", null);
                    var (os, arch, type) = FileNameParser.Parse(relLinkName, relLinkUrl);
                    arts.Add(new Artifact(relLinkName, os, arch, type, relLinkUrl));
                }
                if (arts.Count < 1)
                    continue;
                var release = new Release(relId, relName, arts);
                yield return release;
            }
        }
    }
}