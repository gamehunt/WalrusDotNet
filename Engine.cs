using Antlr4.Runtime;
using Walrus.Lang;

namespace Walrus;

public class RollEngine {
    public class ParseResult {
        public bool Valid { get; set; } = true;
        public string Content { get; set; } = string.Empty;
    }

    private static Random _random = new Random();

    public static List<int> RollDice(int sides, int amount) {
        List<int> r = [];
        for(int i = 0; i < amount; i++) {
            r.Add(_random.Next(1, sides + 1));
        }
        return r;
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