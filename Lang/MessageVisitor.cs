using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Walrus.Lang;

public class MessageVisitor : DiceLangParserBaseVisitor<string> {
    private string _label;
    private readonly ExpressionEvaluatorVisitor _exprEvaluator;
    public MessageVisitor() {
        _label = string.Empty;
        _exprEvaluator = new ExpressionEvaluatorVisitor();
    }

    protected override string DefaultResult => string.Empty;

    protected override string AggregateResult(string aggregate, string nextResult)
    {
        return aggregate + nextResult;
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
        int amount = 1;
        if(context.amount != null) {
            amount = int.Parse(context.amount.Text);
        }
        int sides = int.Parse(context.sides.Text);
        return $"[{string.Join(", ", _exprEvaluator._diceCache[context.DICE_MARKER().Symbol.TokenIndex])}] {amount}d{sides} ";
    }

    public override string VisitLabel(DiceLangParser.LabelContext context)
    {
        _label = context.GetText();
        return string.Empty;
    }

    public override string VisitMessage(DiceLangParser.MessageContext context)
    {
        string content = base.VisitMessage(context);
        if(_label.Length > 0) {
            return _label + ", " + content;
        } else {
            return content;
        }
    }

    public override string VisitValid_expression(DiceLangParser.Valid_expressionContext context)
    {
        DiceLangParser.ExprContext expr = context.expr();
        if(expr != null) {
            double result = _exprEvaluator.Visit(expr);
            return $"{result} <- {base.VisitValid_expression(context)}";
        }
        return base.VisitValid_expression(context);
    }
}