namespace RazorTypescript
{
    public class ElementByDataAttribute : IConvertable
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }

        public string GetTypescriptProperties() => $"    public {Name}: JQuery<HTMLElement>;";
        public string InitTypescriptPropertieName() => $"var {Name} : string;";
        public string InitTypescriptProperties() => $"        this.{Name} = $(\"[{Attribute}]\");";
        public string InitViewProperties() => $"    var {Name} = \"{Attribute}\";";
    }
}
