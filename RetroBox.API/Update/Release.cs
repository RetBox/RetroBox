using System.Collections.Generic;

namespace RetroBox.API.Update
{
    public record Release(string Id, string Name, List<Artifact> Artifacts)
        : IHasId;
}