namespace Walrus.Engine;

public class ListState : IRollElement
{
    public int Amount { get; set; } = 0;
    public bool Unique { get; set; } = false;
    public List<string> Items { get; set; } = [];
    public List<string> Selected { get; set; } = [];
    public string Prettify()
    {
        return $"` {string.Join("; ", Selected)} ` ⟵ {Amount}{(Unique ? 'u' : "")}l[{string.Join("; ", Items)}]";
    }
}