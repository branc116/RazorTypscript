namespace RazorTypescript
{
    public interface IConvertable
    {
        string InitTypescriptPropertieName();
        string GetTypescriptProperties();
        string InitTypescriptProperties();
        string InitViewProperties();
    }
}
