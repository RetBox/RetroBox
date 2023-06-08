using System.Collections.Generic;
using RetroBox.API.Data;

namespace RetroBox.API.Xplat
{
    public interface IPlatExec
    {
        IEnumerable<FoundExe> FindExe(string folder);

        IEnumerable<FoundRom> FindRom(string folder);
    }
}