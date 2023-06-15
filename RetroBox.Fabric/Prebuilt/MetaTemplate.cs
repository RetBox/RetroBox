namespace RetroBox.Fabric.Prebuilt
{
    public sealed class MetaTemplate : IMetaTemplate
    {
        private readonly Template _temp;

        public MetaTemplate(Template temp)
        {
            _temp = temp;
        }

        public string Name => _temp.Name;
    }
}