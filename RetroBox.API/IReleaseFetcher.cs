using System.Collections.Generic;
using RetroBox.API.Update;

namespace RetroBox.API
{
    public interface IReleaseFetcher
    {
        IAsyncEnumerable<Release> FetchEmuReleases();

        IAsyncEnumerable<Release> FetchRomReleases();
    }
}