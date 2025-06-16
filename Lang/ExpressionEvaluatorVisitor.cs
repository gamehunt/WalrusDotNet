using Walrus.Engine;

namespace Walrus.Lang;

public class ExpressionEvaluatorVisitor: DiceLangParserBaseVisitor<double> {
    public ExpressionEvaluatorVisitor() {
        _diceCache = [];
    }

    private Dictionary<int, DiceState> _diceCache;

    protected override double DefaultResult => 0;

    protected override double AggregateResult(double aggregate, double nextResult)
    {
        return aggregate + nextResult;
    }

    public DiceState getDiceFromCache(int tokenIndex) {
        return _diceCache[tokenIndex];
    }

    public void Reset() {
        _diceCache.Clear();
    }

    public override double VisitParentExpr(DiceLangParser.ParentExprContext context)
    {
        return Visit(context.expr());
    }

    public override double VisitAddExpr(DiceLangParser.AddExprContext context)
    {
        double left = Visit(context.expr(0));
        double right = Visit(context.expr(1));
        double result = 0;

        if(context.PLUS() != null) {
            result = left + right;
        } else if (context.MINUS() != null) {
            result = left - right;
        }

        return result;
    }

    public override double VisitMulExpr(DiceLangParser.MulExprContext context)
    {
        double left = Visit(context.expr(0));
        double right = Visit(context.expr(1));
        double result = 0;

        if(context.MUL() != null) {
            result = left * right;
        } else if (context.DIV() != null) {
            result = left / right;
        }

        return result;
    }

    public override double VisitPowExpr(DiceLangParser.PowExprContext context)
    {
        double left = Visit(context.expr(0));
        double right = Visit(context.expr(1));

        return Math.Pow(left, right);
    }

    public override double VisitUnaryExpr(DiceLangParser.UnaryExprContext context)
    {
        double r = base.VisitUnaryExpr(context);
        if(context.MINUS() != null) {
            return -r;
        } else {
            return r;
        }
    }

    public override double VisitDice(DiceLangParser.DiceContext context)
    {
        int index = context.DICE().Symbol.TokenIndex;
        int amount = 1;
        if(context.amount != null) {
            amount = int.Parse(context.amount.Text);
        }
        int sides = int.Parse(context.sides.Text);
        DiceState state = new();
        state.Amount = amount;
        state.Sides = sides;
        state.Subresults = RollEngine.RollDice(sides, amount);
        DiceLangParser.Dice_discard_groupContext discardGroup = context.dice_discard_group();
        if(discardGroup != null) {
            state.Discard = int.Parse(discardGroup.amount.Text);
            if(discardGroup.discard == null || discardGroup.discard.Type == DiceLangLexer.LIST) {
                state.Discard = -state.Discard;
            }
        }
        for(int i = 0; i < amount; i++) {
            if(state.Discard == 0 ||
               state.Discard < 0 && amount + state.Discard > i || 
               state.Discard > 0 && i >= state.Discard) {
                state.Result += state.Subresults[i];
            }
        }
        _diceCache.Add(index, state);
        return state.Result;
    }

    public override double VisitNumeric(DiceLangParser.NumericContext context)
    {
        return double.Parse(context.GetText());
    }

}