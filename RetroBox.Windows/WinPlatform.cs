﻿using RetroBox.API.Xplat;

namespace RetroBox.Windows
{
    public sealed class WinPlatform : IPlatform
    {
        public IPlatFolder Folders { get; } = new WinFolders();
    }
}