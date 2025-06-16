using Antlr4.Runtime;
using System.Linq;
using Walrus.Engine;
using Walrus.Lang;

namespace Walrus;

public class RollEngine {
    public class ParseResult {
        public bool Valid { get; set; } = true;
        public string Content { get; set; } = string.Empty;
    }

    private static readonly Random _random = new();

    public static List<int> RollDice(int sides, int amount) {
        List<int> r = [];
        for(int i = 0; i < amount; i++) {
            r.Add(_random.Next(1, sides + 1));
        }
        r = [.. r.OrderDescending()];
        return r;
    }

    public static List<string> SelectFromList(int amount, bool unique, List<string> list) {
        if(unique) {
            return list.OrderBy(x => Guid.NewGuid()).Take(amount).ToList();
        } else {
            List<string> results = [];
            for(int i = 0; i < amount; i++) {
                results.Add(list[_random.Next(list.Count)]);
            }
            return results;
        }
    }

    public static List<ContestastState> RollInitiative(List<ContestastState> contestants) {
        foreach(ContestastState contestant in contestants) {
            if(contestant.Advantage) {
                contestant.Subresults = RollDice(20, 2);
                contestant.Result = contestant.Subresults.Max();
            } else if (contestant.Disadvantage) {
                contestant.Subresults = RollDice(20, 2);
                contestant.Result = contestant.Subresults.Min();
            } else {
                contestant.Subresults = RollDice(20, 1);
                contestant.Result = contestant.Subresults[0];
            }
            contestant.Subresults = contestant.Subresults.Select(t => t + contestant.Mod).ToList();
            contestant.Result += contestant.Mod;
        }
        return [.. contestants.OrderByDescending(c => c.Result).ThenByDescending(c => c.Mod)];
    }

    public static ParseResult Parse(string content) {
        ICharStream stream = CharStreams.fromString(content);
        ITokenSource lexer = new DiceLangLexer(stream, null, null);
        ITokenStream tokens = new CommonTokenStream(lexer);
        DiceLangParser parser = new DiceLangParser(tokens, null, null);
        parser.ErrorHandler = new BailErrorStrategy();
        ParseResult res = new();
        try {
            DiceLangParser.MessageContext ctx = parser.message();
            MessageVisitor visitor = new MessageVisitor();
            res.Content = visitor.Visit(ctx);
            res.Valid = visitor.HasDice();
        } catch {
            res.Valid = false;
        }
        return res;
    }

    public static string Evaluate(string content) {
        var r = Parse(content);
        if(r.Valid) {
            return r.Content;
        }
        return string.Empty;
    }
}