namespace AcGridGeneratorUi
{
    public class CarListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
