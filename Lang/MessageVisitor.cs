using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Walrus.Engine;

namespace Walrus.Lang;

public class MessageVisitor : DiceLangParserBaseVisitor<string> {
    private string _label;
    private readonly ExpressionEvaluatorVisitor _exprEvaluator;
    private bool _hasDice;
    public MessageVisitor() {
        _label = string.Empty;
        _hasDice = false;
        _exprEvaluator = new ExpressionEvaluatorVisitor();
    }

    protected override string DefaultResult => string.Empty;

    protected override string AggregateResult(string aggregate, string nextResult)
    {
        return aggregate + nextResult;
    }

    public bool HasDice() {
        return _hasDice;
    }

    public override string VisitTerminal(ITerminalNode context)
    {
        if(context.Symbol.Type == DiceLangLexer.Eof) {
            return string.Empty;
        }
        string postfix = string.Empty;
        if(context.Symbol.Type != DiceLangLexer.LBRACE && context.Symbol.Type != DiceLangLexer.RBRACE) {
            postfix = " ";
        }
        return context.GetText() + postfix;
    }

    public override string VisitDice(DiceLangParser.DiceContext context)
    {
        _hasDice = true;
        int index = context.DICE().Symbol.TokenIndex;
        DiceState state = _exprEvaluator.getDiceFromCache(index);
        return state.Prettify();
    }

    public override string VisitLabel(DiceLangParser.LabelContext context)
    {
        _label = context.GetText();
        return string.Empty;
    }

    public override string VisitMessage(DiceLangParser.MessageContext context)
    {
        bool forceMarker = false;

        if(context.FORCE_CALC_MARKER() != null) {
            _hasDice = true;
            forceMarker = true;
        }

        string content = base.VisitMessage(context);

        if(forceMarker) {
            content = content.Substring(1);
        }

        if(_label.Length > 0) {
            string[] lines = content.Split("\n");
            content = "";
            foreach(string line in lines) {
                if(line.Length > 0) {
                    content += $"{_label}, {line}\n";
                }
            }
        } 

        return content;
    }

    public override string VisitValid_expression(DiceLangParser.Valid_expressionContext context)
    {
        DiceLangParser.ExprContext expr = context.expr();
        if(expr != null) {
            double result = _exprEvaluator.Visit(expr);
            return $"` {result} ` ⟵ {base.VisitValid_expression(context)}";
        }
        return base.VisitValid_expression(context);
    }

    public override string VisitRepeated_expr(DiceLangParser.Repeated_exprContext context)
    {
        string result = string.Empty;
        int repeats = int.Parse(context.amount.Text);
        for(int i = 0; i < repeats; i++) {
            DiceLangParser.ExprContext expr = context.expr();
            double exprResult = _exprEvaluator.Visit(expr);
            result += $"` {exprResult} ` ⟵ {Visit(expr)}\n";
            _exprEvaluator.Reset();
        }
        return result;
    }

    public override string VisitInitiative(DiceLangParser.InitiativeContext context)
    {
        _hasDice = true;
        List<ContestastState> contestants = [];
        foreach(var contestant in context._contestants) {
            ContestastState parsedContestant = new();
            string rawName = contestant.name.Text;
            parsedContestant.Name = rawName.Substring(1, rawName.Length - 2);
            parsedContestant.Mod = int.Parse(contestant.mod.Text);
            if(contestant.mod_sign.Type == DiceLangLexer.MINUS) {
                parsedContestant.Mod = -parsedContestant.Mod;
            }
            if(contestant.advantage != null) {
                parsedContestant.Advantage = contestant.advantage.Type == DiceLangLexer.PLUS;
                parsedContestant.Disadvantage = contestant.advantage.Type == DiceLangLexer.MINUS;
            }
            contestants.Add(parsedContestant);
        }
        contestants = RollEngine.RollInitiative(contestants);
        List<string> result = [];
        foreach(ContestastState contestant in contestants) {
            result.Add(contestant.Prettify());
        }
        return $"__{string.Join(" - ", result)}__";
    }

    public override string VisitList([NotNull] DiceLangParser.ListContext context)
    {
        _hasDice = true;
        int amount = 1;
        if(context.amount != null) {
            amount = int.Parse(context.amount.Text);
        }
        bool uniq = context.UNIQ() != null;
        List<string> items = context._items.Select(t => t.Text).ToList();
        ListState state = new();
        state.Amount = amount;
        state.Unique = uniq;
        state.Items  = items;
        state.Selected = RollEngine.SelectFromList(amount, uniq, items);
        return state.Prettify();
    }
}