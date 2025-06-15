namespace Walrus.Lang;

public class ExpressionEvaluatorVisitor: DiceLangParserBaseVisitor<double> {
    public ExpressionEvaluatorVisitor() {
        _diceCache = [];
    }

    public Dictionary<int, List<int>> _diceCache;

    protected override double DefaultResult => 0;

    protected override double AggregateResult(double aggregate, double nextResult)
    {
        return aggregate + nextResult;
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
        int index = context.DICE_MARKER().Symbol.TokenIndex;
        double r = 0;
        int amount = 1;
        if(context.amount != null) {
            amount = int.Parse(context.amount.Text);
        }
        int sides = int.Parse(context.sides.Text);
        _diceCache.Add(index, RollEngine.RollDice(sides, amount));
        for(int i = 0; i < amount; i++) {
            r += _diceCache[index][i];
        }
        return r;
    }

    public override double VisitNumeric(DiceLangParser.NumericContext context)
    {
        return double.Parse(context.GetText());
    }

}