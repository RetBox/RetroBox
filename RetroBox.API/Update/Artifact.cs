namespace RetroBox.API.Update
{
    public record Artifact(string Name, OsName OS, OsArch[] Arch, FileType Type, string Url);
}