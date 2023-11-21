using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace HULKLibrary;

/// <summary>
/// Abstract superclass that represents all Expressions
/// </summary>
public abstract class Expression
{
    /// <summary>
    /// Class for representing Literals
    /// </summary>
    public class Literal : Expression
    {
        /// <summary>
        /// Literal Constructor
        /// </summary>
        /// <param name="value">Value of the literal</param>
        /// <param name="type">Type of the literal</param>
        public Literal(object value, DataType type = DataType.NULL)
        {
            this.value = value;
            this.type = type;
        }

        /// <summary>
        /// Value of the literal
        /// </summary>
        public readonly object value;
        /// <summary>
        /// Type of the literal
        /// </summary>
        private readonly DataType type;
    }

    /// <summary>
    /// Class for representing Unary Expressions
    /// </summary>
    public class Unary : Expression
    {
        /// <summary>
        /// Unary Expression Constructor
        /// </summary>
        /// <param name="token">Token that contains the operation to apply</param>
        /// <param name="right">Expression to apply the operation</param>
        public Unary(Token token, Expression right)
        {
            this.token = token;
            this.right = right;
        }

        /// <summary>
        /// Method for evaluating Unary Expressions
        /// </summary>
        /// <param name="value">Object to evaluate</param>
        /// <returns>An object with the result of the evaluation</returns>
        /// <exception cref="Error"></exception>
        public object Eval(object value)
        {
            switch (token.type)
            {
                case TokenType.MINUS:
                    if (Utils.OperandIsNumber(value))
                        return -(double)value;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be number", token);
                case TokenType.BANG:
                    if (Utils.OperandIsBool(value))
                        return !(bool)value;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operand must be bool", token);
            }

            return null!;
        }

        /// <summary>
        /// Token that contains the operation
        /// </summary>
        private readonly Token token;
        /// <summary>
        /// Expression to apply the operation
        /// </summary>
        public readonly Expression right;
    }

    /// <summary>
    /// Variable Expression Class
    /// </summary>
    public class Variable : Expression
    {
        /// <summary>
        /// Variable Constructor
        /// </summary>
        /// <param name="name"></param>
        public Variable(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Name of the variable
        /// </summary>
        public string name;
        //private readonly object value = null!;
    }

    /// <summary>
    /// Assign Expression Class
    /// </summary>
    public class Assign : Expression
    {
        /// <summary>
        /// Assign Constructor
        /// </summary>
        /// <param name="identifier">Name of the variable</param>
        /// <param name="value">Assigned Value</param>
        public Assign(string identifier, Expression value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        /// <summary>
        /// Name of the variable
        /// </summary>
        public readonly string identifier;
        /// <summary>
        /// Assigned value
        /// </summary>
        public readonly Expression value;
    }

    /// <summary>
    /// Binary Expression Class
    /// </summary>
    public class Binary : Expression
    {
        /// <summary>
        /// Binary Constructor
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="oper">Operation</param>
        /// <param name="right">Right operand</param>
        public Binary(Expression left, Token oper, Expression right)
        {
            this.left = left;
            this.oper = oper;
            this.right = right;
        }

        /// <summary>
        /// Method for evaluating Binary Expressions
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>An object with the result of the evaluation</returns>
        /// <exception cref="Error"></exception>
        public object Eval(object left, object right)
        {
            switch (oper.type)
            {
                case TokenType.PLUS:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left + (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.MINUS:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left - (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.STAR:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left * (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.SLASH:
                    if (Utils.OperandsAreNumbers(left, right))
                    {
                        if ((double)right == 0)
                            throw new Error(ErrorType.SEMANTIC_ERROR, "Division by zero", oper);
                        return (double)left / (double)right;
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.MOD:
                    if (Utils.OperandsAreNumbers(left, right))
                    {
                        if ((double)right == 0)
                            throw new Error(ErrorType.SEMANTIC_ERROR, "Modulo by zero", oper);
                        return (double)(Convert.ToInt64((double)left) % Convert.ToInt64((double)right));
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.CARET:
                    if (Utils.OperandsAreNumbers(left, right))
                        return Math.Pow((double)left, (double)right);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.CONCAT:
                    string leftS = left is Boolean ? left.ToString()!.ToLower() : left.ToString()!;
                    string rightS = right is Boolean ? right.ToString()!.ToLower() : right.ToString()!;
                    return leftS + rightS;
                case TokenType.AMPERSAND:
                    if (Utils.OperandsAreBool(left, right))
                        return (bool)left && (bool)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be boolean", oper);
                case TokenType.VER_BAR:
                    if (Utils.OperandsAreBool(left, right))
                        return (bool)left || (bool)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be boolean", oper);
                case TokenType.EQUAL_EQUAL:
                    return Utils.IsEqual(left, right);
                case TokenType.BANG_EQUAL:
                    return !Utils.IsEqual(left, right);
                case TokenType.LESS_EQUAL:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left <= (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.GREATER_EQUAL:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left >= (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.GREATER:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left > (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
                case TokenType.LESS:
                    if (Utils.OperandsAreNumbers(left, right))
                        return (double)left < (double)right;
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Operands must be numbers", oper);
            }

            return null!;
        }

        /// <summary>
        /// Left operand
        /// </summary>
        public readonly Expression left;
        /// <summary>
        /// Right operand
        /// </summary>
        public readonly Expression right;
        /// <summary>
        /// Operation to be made
        /// </summary>
        private readonly Token oper;
    }

    /// <summary>
    /// Call Expression Class
    /// </summary>
    public class Call : Expression
    {
        /// <summary>
        /// Expression Constructor
        /// </summary>
        /// <param name="identifier">Name of the function</param>
        /// <param name="arguments">List of arguments</param>
        /// <param name="function">Function expression</param>
        public Call(string identifier, List<Expression> arguments, Function function)
        {
            this.function = function;
            this.identifier = identifier;
            this.arguments = arguments;
        }

        /// <summary>
        /// Method for evaluating Function Calls
        /// </summary>
        /// <param name="context">Variables context to evaluate the function call</param>
        /// <returns>An object with the result of the evaluated expression</returns>
        /// <exception cref="Error"></exception>
        public object Eval(Dictionary<string, object> context)
        {
            List<object> args = new List<object>();
            foreach (Expression arg in arguments)
            {
                args.Add(Evaluate.GetValue(arg, context));
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
                    if (Utils.OperandIsNumber(args[0]))
                        return Math.Sqrt((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "sin":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'sin' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.OperandIsNumber(args[0]))
                        return Math.Sin((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "cos":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'cos' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.OperandIsNumber(args[0]))
                        return Math.Cos((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "exp":
                    if (args.Count != 1)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'exp' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.OperandIsNumber(args[0]))
                        return Math.Exp((double)args[0]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "A number was expected, instead a " + args[0].GetType() + " was given", Scanner.line);
                case "log":
                    if (args.Count != 2)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'log' receives 1 argument(s), but " + context.Count + "were given", Scanner.line);
                    if (Utils.OperandsAreNumbers(args[0], args[1]))
                        return Math.Log((double)args[0], (double)args[1]);
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Numbers were expected, instead " + args[0].GetType() + " and " + args[1].GetType() + " were given", Scanner.line);
                case "rand":
                    if (args.Count != 0)
                        throw new Error(ErrorType.SEMANTIC_ERROR, "Function 'rand' takes 0 arguments", Scanner.line);
                    Random rnd = new Random();
                    return rnd.NextDouble();
                default:
                    function = Functions.Get(identifier).copy();
                    if (args.Count == function.arguments.Count)
                    {
                        int cnt = 0;

                        foreach (string name in function.arguments)
                        {
                            function.value[name] = args[cnt++];
                        }

                        return Evaluate.GetValue(function.body, function.value);
                    }
                    throw new Error(ErrorType.SEMANTIC_ERROR, "Function " + identifier + " receives " + function.arguments.Count + " but " + args.Count + " were given", Scanner.line);
            }
        }

        /// <summary>
        /// Function name
        /// </summary>
        private readonly string identifier;
        /// <summary>
        /// Function arguments
        /// </summary>
        public List<Expression> arguments;
        /// <summary>
        /// Function Expression
        /// </summary>
        private Function function;
    }

    /// <summary>
    /// Function Declaration Class
    /// </summary>
    public class Function : Expression
    {
        /// <summary>
        /// Function Constructor
        /// </summary>
        /// <param name="identifier">Name of the function</param>
        /// <param name="arguments">List of arguments</param>
        /// <param name="body">Body of the function</param>
        public Function(string identifier, List<string> arguments, Expression body)
        {
            this.identifier = identifier;
            this.arguments = arguments;
            this.body = body;
        }

        /// <summary>
        /// Method to copy the function
        /// </summary>
        /// <returns>A Function wich is a copy of the current function</returns>
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
        
        /// <summary>
        /// Function name
        /// </summary>
        private readonly string identifier;
        /// <summary>
        /// Function Arguments
        /// </summary>
        public readonly List<string> arguments;
        /// <summary>
        /// Function context
        /// </summary>
        public readonly Dictionary<string, object> value = new Dictionary<string, object>();
        /// <summary>
        /// Function body
        /// </summary>
        public readonly Expression body;
    }

    /// <summary>
    /// Grouping Expression Class
    /// </summary>
    public class Grouping : Expression
    {
        /// <summary>
        /// Grouping Constructor
        /// </summary>
        /// <param name="expr"></param>
        Grouping(Expression expr)
        {
            this.expr = expr;
        }

        /// <summary>
        /// Expression inside grouping
        /// </summary>
        private readonly Expression expr;
    }

    /// <summary>
    /// If-Else Statement Class
    /// </summary>
    public class IfStatement : Expression
    {
        /// <summary>
        /// If-Else Statement Constructor
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="ifBody">If body</param>
        /// <param name="elseBody">Else body</param>
        public IfStatement(Expression condition, Expression ifBody, Expression elseBody)
        {
            this.condition = condition;
            this.ifBody = ifBody;
            this.elseBody = elseBody;
        }

        /// <summary>
        /// CheckValue helper for detecting errors in both branches of if-else
        /// </summary>
        /// <param name="context">Context for evaluating</param>
        public void checkValue(Dictionary<string, object> context)
        {
            object ifBdy = Evaluate.GetValue(ifBody, context);
            object elseBdy = Evaluate.GetValue(elseBody, context);
        }

        /// <summary>
        /// Method for evaluating If-Else expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns>An object with the result of the evaluated expression</returns>
        /// <exception cref="Error"></exception>
        public object Eval(Dictionary<string, object> context)
        {
            if (Evaluate.GetValue(condition, context) is bool check)
            {
                if (check)
                    return Evaluate.GetValue(ifBody, context);
                else
                    return Evaluate.GetValue(elseBody, context);
            }

            throw new Error(ErrorType.SEMANTIC_ERROR, "Condition must return a boolean");
        }

        /// <summary>
        /// If condition
        /// </summary>
        private readonly Expression condition;
        /// <summary>
        /// If Body
        /// </summary>
        private readonly Expression ifBody;
        /// <summary>
        /// Else Body
        /// </summary>
        private readonly Expression elseBody;
    }

    /// <summary>
    /// Let Statement Class
    /// </summary>
    public class LetStatement : Expression
    {
        /// <summary>
        /// Let Constructor
        /// </summary>
        /// <param name="assignBody">List of variable assignements</param>
        /// <param name="body">Expression body</param>
        public LetStatement(List<Assign> assignBody, Expression body)
        {
            this.assignBody = assignBody;
            this.body = body;
        }

        /// <summary>
        /// Method for evaluating Let
        /// </summary>
        /// <param name="context">Context for evaluating</param>
        /// <returns>An object with the result of the evaluated expression</returns>
        public object Eval(Dictionary<string, object> context)
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
                variable[assign.identifier] = Evaluate.GetValue(assign.value, variable);
            }

            return Evaluate.GetValue(body, variable);
        }

        /// <summary>
        /// Assignements Body
        /// </summary>
        private readonly List<Assign> assignBody;
        /// <summary>
        /// Expression Body
        /// </summary>
        private readonly Expression body;
    }
}