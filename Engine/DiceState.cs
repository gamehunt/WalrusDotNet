namespace Walrus.Engine;

public class DiceState : IRollElement {
    public List<int> Subresults { get; set; } = [];
    public int Result { get; set; } = 0;

    public int Amount { get; set; } = 0;
    public int Sides { get; set; } = 0;

    public int Discard { get; set; } = 0;

    public string Prettify() {
        List<string> formattedList = [];
        for(int i = 0; i < Subresults.Count; i++) {
            int r = Subresults[i];
            string formatted = $"{r}";
            if(r == Sides || r == 1) {
                formatted = $"**{r}**";
            } 
            if(Discard < 0 && Subresults.Count + Discard <= i ||
               Discard > 0 && i < Discard) {
                formatted = $"~~{formatted}~~";
            }
            formattedList.Add(formatted);
        }
        string list = string.Join(", ", formattedList);
        string postfix = string.Empty;
        if(Discard < 0) {
            postfix = $"dl{-Discard}";
        } else if (Discard > 0) {
            postfix = $"dh{Discard}";
        }
        if(Amount == 1) {
            return $"[{list}] {Amount}d{Sides}{postfix}";
        } else {
            return $"[{list}] ({Result}) {Amount}d{Sides}{postfix}";
        }
    }

}