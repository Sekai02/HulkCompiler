namespace HULKLibrary;

public class Evaluate
{
    private readonly Expression Ast;

    public Evaluate(Expression Ast)
    {
        this.Ast = Ast;
    }

    public object run()
    {
        try
        {
            return getValue(Ast, new Dictionary<string, object>());
        }
        catch (Error error)
        {
            error.Report();
            return null;
        }
    }

    public static object getValue(Expression expr, Dictionary<string, object> value)
    {
        Utils.callCount++;
        if (Utils.callCount > Utils.stackLimit)
            throw new StackOverflowException();

        switch (expr)
        {
            case Expression.Literal:
                return ((Expression.Literal)expr).value;
            case Expression.Variable:
                Expression.Variable variable = (Expression.Variable)expr;
                if (!value.ContainsKey(variable.name))
                    throw new Error(ErrorType.SEMANTIC_ERROR, variable.name + " is no defined.", Scanner.line);
                return value[variable.name];
            case Expression.Binary:
                Expression.Binary binary = (Expression.Binary)expr;
                return binary.eval(getValue(binary.left, value), getValue(binary.right, value));
            case Expression.Unary:
                Expression.Unary unary = (Expression.Unary)expr;
                return unary.eval(getValue(unary.right, value));
            case Expression.Call:
                Expression.Call call = (Expression.Call)expr;
                return call.eval(value);
            case Expression.Function:
                return "Successful function declaration.";
            case Expression.IfStatement:
                Expression.IfStatement ifstmt = (Expression.IfStatement)expr;
                return ifstmt.eval(value);
            case Expression.LetStatement:
                Expression.LetStatement let = (Expression.LetStatement)expr;
                return let.eval(value);
            default:
                return null;
        }
    }
}