namespace Walrus.Engine;

public class ContestastState : IRollElement {
    public string Name { get; set; } = string.Empty;
    public int Mod { get; set; } = 0;
    public bool Advantage { get; set; } = false;
    public bool Disadvantage { get; set; } = false;
    public int Result { get; set; } = 0;
    public List<int> Subresults { get; set; } = [];

    public string Prettify()
    {
        if(Subresults.Count > 1) {
            return $"{Name} ({Result} ⟵ [{string.Join(", ", Subresults)}])";
        } else {
            return $"{Name} ({Result})";
        }
    }
}