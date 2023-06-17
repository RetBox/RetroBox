using CliWrap.EventStream;

namespace RetroBox.Manager.CoreLogic
{
    public delegate void MonitorHandler(object sender, string tag, CommandEvent evt);
}