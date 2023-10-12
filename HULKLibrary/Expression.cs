using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace HULKLibrary;

public abstract class Expression
{
    public class Literal : Expression
    {
        public Literal(object value, DataType type = DataType.NULL)
        {
            this.value = value;
            this.type = type;
        }

        public readonly object value;
        private readonly DataType type;
    }

    public class Unary : Expression
    {
        public Unary(Token token, Expression right)
        {
            this.token = token;
            this.right = right;
        }

        public object eval(object value)
        {
            switch (token.type)
            {
                case TokenType.MINUS:
                    if (Utils.operandIsNumber(value))
                        return -(double)value;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be number", token);
                case TokenType.BANG:
                    if (Utils.operandIsBool(value))
                        return !(bool)value;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operand must be bool", token);
            }

            return null;
        }

        private readonly Token token;
        public readonly Expression right;
    }

    public class Variable : Expression
    {
        public Variable(string name)
        {
            this.name = name;
        }

        public string name;
        private readonly object value = null;
    }

    public class Assign : Expression
    {
        public Assign(string identifier, Expression value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public readonly string identifier;
        public readonly Expression value;
    }

    public class Binary : Expression
    {
        public Binary(Expression left, Token oper, Expression right)
        {
            this.left = left;
            this.oper = oper;
            this.right = right;
        }

        public object eval(object left, object right)
        {
            switch (oper.type)
            {
                case TokenType.PLUS:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left + (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.MINUS:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left - (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.STAR:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left * (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.SLASH:
                    if (Utils.operandsAreNumbers(left, right))
                    {
                        if ((double)right == 0)
                            throw new Error(ErrorType.SEMANTIC_ERROR, "Division by zero", oper);
                        return (double)left / (double)right;
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.MOD:
                    if (Utils.operandsAreNumbers(left, right))
                    {
                        if ((double)right == 0)
                            throw new Error(ErrorType.SEMANTIC_ERROR, "Modulo by zero", oper);
                        return (double)(Convert.ToInt64((double)left) % Convert.ToInt64((double)right));
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.CARET:
                    if (Utils.operandsAreNumbers(left, right))
                        return Math.Pow((double)left, (double)right);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.CONCAT:
                    string leftS = left is Boolean ? left.ToString().ToLower() : left.ToString();
                    string rightS = right is Boolean ? right.ToString().ToLower() : right.ToString();
                    return leftS + rightS;
                case TokenType.AMPERSAND:
                    if (Utils.operandsAreBool(left, right))
                        return (bool)left && (bool)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be boolean", oper);
                case TokenType.VER_BAR:
                    if (Utils.operandsAreBool(left, right))
                        return (bool)left || (bool)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be boolean", oper);
                case TokenType.EQUAL_EQUAL:
                    return Utils.isEqual(left, right);
                case TokenType.BANG_EQUAL:
                    return !Utils.isEqual(left, right);
                case TokenType.LESS_EQUAL:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left <= (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.GREATER_EQUAL:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left >= (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.GREATER:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left > (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.LESS:
                    if (Utils.operandsAreNumbers(left, right))
                        return (double)left < (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
            }

            return null;
        }

        public readonly Expression left;
        public readonly Expression right;
        private readonly Token oper;
    }

    public class Call : Expression
    {
        public Call(string identifier, List<Expression> arguments, Function function)
        {
            this.function = function;
            this.identifier = identifier;
            this.arguments = arguments;
        }

        public object eval(Dictionary<string, object> context)
        {
            List<object> args = new List<object>();
            foreach (Expression arg in arguments)
            {
                args.Add(Evaluate.getValue(arg, context));
            }

            switch (identifier)
            {
                case "print":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'print' receives 1 argument(s), but " + context.Count + " were given", Scanner.line);
                    return args[0];
                case "sqrt":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'sqrt' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.operandIsNumber(args[0]))
                        return Math.Sqrt((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "sin":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'sin' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.operandIsNumber(args[0]))
                        return Math.Sin((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "cos":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'cos' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.operandIsNumber(args[0]))
                        return Math.Cos((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "exp":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'exp' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.operandIsNumber(args[0]))
                        return Math.Exp((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "log":
                    if (args.Count != 2)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'log' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.operandsAreNumbers(args[0], args[1]))
                        return Math.Log((double)args[0], (double)args[1]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Numbers were expected, instead " + args[0].GetType() + " and " + args[1].GetType() + " were given", Scanner.line);
                case "rand":
                    if (args.Count != 0)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'rand' takes 0 arguments", Scanner.line);
                    Random rnd = new Random();
                    return rnd.NextDouble();
                default:
                    function = Functions.get(identifier).copy();
                    if (args.Count == function.arguments.Count)
                    {
                        int cnt = 0;

                        foreach (string name in function.arguments)
                        {
                            function.value[name] = args[cnt++];
                        }

                        return Evaluate.getValue(function.body, function.value);
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Function " + identifier + " receives " + function.arguments.Count + " but " + args.Count + " were given", Scanner.line);
            }
        }

        private readonly string identifier;
        public List<Expression> arguments;
        private Function function;
    }

    public class Function : Expression
    {
        public Function(string identifier, List<string> arguments, Expression body)
        {
            this.identifier = identifier;
            this.arguments = arguments;
            this.body = body;
        }

        public Function copy()
        {
            Function newFunction = new Function(identifier, arguments, body);
            if (value != null)
            {
                foreach (string key in value.Keys)
                {
                    newFunction.value[key] = value[key];
                }
            }

            return newFunction;
        }

        private readonly string identifier;
        public readonly List<string> arguments;
        public readonly Dictionary<string, object> value = new Dictionary<string, object>();
        public readonly Expression body;
    }

    public class Grouping : Expression
    {
        Grouping(Expression expr)
        {
            this.expr = expr;
        }
        private readonly Expression expr;
    }

    public class IfStatement : Expression
    {
        public IfStatement(Expression condition, Expression ifBody, Expression elseBody)
        {
            this.condition = condition;
            this.ifBody = ifBody;
            this.elseBody = elseBody;
        }

        public object eval(Dictionary<string, object> context)
        {
            if (Evaluate.getValue(condition, context) is bool check)
            {
                if (check)
                    return Evaluate.getValue(ifBody, context);
                else
                    return Evaluate.getValue(elseBody, context);
            }

            throw new Error(ErrorType.SEMANTIC_ERROR, "Condition must return a boolean");
        }

        private readonly Expression condition;
        private readonly Expression ifBody;
        private readonly Expression elseBody;
    }

    public class LetStatement : Expression
    {
        public LetStatement(List<Assign> assignBody, Expression body)
        {
            this.assignBody = assignBody;
            this.body = body;
        }

        public object eval(Dictionary<string, object> context)
        {
            Dictionary<string, object> variable = new Dictionary<string, object>();

            if (context != null)
            {
                foreach (string key in context.Keys)
                {
                    variable[key] = context[key];
                }
            }

            foreach (Assign assign in assignBody)
            {
                variable[assign.identifier] = Evaluate.getValue(assign.value, variable);
            }

            return Evaluate.getValue(body, variable);
        }

        private readonly List<Assign> assignBody;
        private readonly Expression body;
    }
}