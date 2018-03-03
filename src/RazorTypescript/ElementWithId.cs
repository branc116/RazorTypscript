namespace RazorTypescript
{
    public class ElementWithId : IConvertable
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string GetTypescriptProperties() => $"    public {Name}: HTMLElement;";
        public string InitTypescriptPropertieName() => $"var {Name} : string;";
        public string InitTypescriptProperties() => $"        this.{Name} = document.getElementById({Name})!;";
        public string InitViewProperties() => $"    var {Name} = \"{Id}\";";
    }
}
