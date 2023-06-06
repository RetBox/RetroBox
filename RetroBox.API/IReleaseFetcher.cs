using RetroBox.API.Update;

namespace RetroBox.API
{
    public interface IReleaseFetcher
    {
        Release[] FetchEmuReleases();

        Release[] FetchRomReleases();
    }
}