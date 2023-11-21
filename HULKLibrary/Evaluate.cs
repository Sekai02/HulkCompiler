namespace HULKLibrary;

/// <summary>
/// Interpreter class
/// </summary>
public class Evaluate
{
    /// <summary>
    /// Abstract Syntax Tree
    /// </summary>
    private readonly Expression Ast;

    /// <summary>
    /// Evaluate(Intepreter) constructor
    /// </summary>
    /// <param name="Ast">Takes the root of the parsed AST</param>
    public Evaluate(Expression Ast)
    {
        this.Ast = Ast;
    }

    /// <summary>
    /// Runs the code interpreting the parsed AST
    /// </summary>
    /// <returns>The returned value of all the proccess</returns>
    public object Run()
    {
        try
        {
            return GetValue(Ast, new Dictionary<string, object>());
        }
        catch (Error error)
        {
            error.Report();
            return null!;
        }
    }

    /// <summary>
    /// Evaluates a given expression under a context
    /// </summary>
    /// <param name="expr">Expression to evaluate</param>
    /// <param name="value">Context to evaluate the expression</param>
    /// <returns>The result of evaluating the expression</returns>
    /// <exception cref="StackOverflowException"></exception>
    /// <exception cref="Error"></exception>
    public static object GetValue(Expression expr, Dictionary<string, object> value)
    {
        Utils.callCount++;
        if (Utils.callCount > Utils.stackLimit)
            throw new Error(ErrorType.SEMANTIC_ERROR, "Stack Overflow",HULK.Line);
        //throw new StackOverflowException();

        switch (expr)
        {
            case Expression.Literal:
                return ((Expression.Literal)expr).value;
            case Expression.Variable:
                Expression.Variable variable = (Expression.Variable)expr;
                if (!value.ContainsKey(variable.name))
                    throw new Error(ErrorType.SEMANTIC_ERROR, variable.name + " is no defined.", HULK.Line);
                return value[variable.name];
            case Expression.Binary:
                Expression.Binary binary = (Expression.Binary)expr;
                return binary.Eval(GetValue(binary.left, value), GetValue(binary.right, value));
            case Expression.Unary:
                Expression.Unary unary = (Expression.Unary)expr;
                return unary.Eval(GetValue(unary.right, value));
            case Expression.Call:
                Expression.Call call = (Expression.Call)expr;
                return call.Eval(value);
            case Expression.Function:
                return "Function declared.";
            case Expression.IfStatement:
                Expression.IfStatement ifstmt = (Expression.IfStatement)expr;
                //ifstmt.checkValue(value);
                return ifstmt.Eval(value);
            case Expression.LetStatement:
                Expression.LetStatement let = (Expression.LetStatement)expr;
                return let.Eval(value);
            default:
                return null!;
        }
    }
}