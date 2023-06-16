using RetroBox.Common;

namespace RetroBox.Manager.Boxes
{
    public static class MachineStates
    {
        public static byte[] GetImage(this MachineState state)
        {
            var text = state.ToString().ToLower();
            var file = $"vm-{text}.png";
            var img = Resources.LoadBytes(file, typeof(MetaMachine));
            return img;
        }

        public static string GetText(this MachineState state)
        {
            var text = state.ToString();
            return text;
        }
    }
}