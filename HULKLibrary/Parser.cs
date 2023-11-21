namespace HULKLibrary;

/// <summary>
/// Parser
/// </summary>
public class Parser
{
    /// <summary>
    /// Token List
    /// </summary>
    private readonly List<Token> tokens;
    /// <summary>
    /// Current pos
    /// </summary>
    int current = 0;

    /// <summary>
    /// Parser constructor
    /// </summary>
    /// <param name="tokens">Token list to consume and build the AST</param>
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    /// <summary>
    /// Get Current token of the list
    /// </summary>
    /// <returns>The current token of the list</returns>
    private Token GetToken()
    {
        return tokens[current];
    }

    /// <summary>
    /// Gets the type of the current token of the list
    /// </summary>
    /// <returns>The type of the current token</returns>
    private new TokenType GetType()
    {
        return tokens[current].type;
    }

    /// <summary>
    /// Gets the lexeme of the current token of the list
    /// </summary>
    /// <returns></returns>
    private string GetLexeme()
    {
        return tokens[current].lexeme;
    }

    /// <summary>
    /// Checks if the current Token matches any of the types
    /// </summary>
    /// <param name="tokentypes">Types to match</param>
    /// <returns>True if matches, False otherwise</returns>
    private bool Match(params TokenType[] tokentypes)
    {
        if (GetType() == TokenType.EOF)
            return false;

        foreach (TokenType type in tokentypes)
            if (GetType() == type)
                return true;

        return false;
    }

    /// <summary>
    /// Advances to the next token and returns the current position
    /// </summary>
    /// <returns>Current pos</returns>
    private int Advance()
    {
        if (GetType() != TokenType.EOF)
            current++;

        return current - 1;
    }

    /// <summary>
    /// Consumes the current token if it matches the current type
    /// </summary>
    /// <param name="type">Type to match</param>
    /// <param name="lexeme">Lexeme</param>
    /// <returns>The current pos</returns>
    /// <exception cref="Error"></exception>
    private int Consume(TokenType type, string lexeme)
    {
        if (Match(type))
            return Advance();

        throw new Error(ErrorType.SYNTAX_ERROR, "Expect '" + lexeme + "'", GetToken());
    }

    /// <summary>
    /// Parse method
    /// </summary>
    /// <returns>Root of the AST</returns>
    public Expression Parse()
    {
        string delete = "";
        try
        {
            if (Match(TokenType.FUNCTION))
            {
                Advance();
                string identifier = "";

                if (Match(TokenType.IDENTIFIER))
                    identifier = GetLexeme();

                Consume(TokenType.IDENTIFIER, "a name");

                Consume(TokenType.LEFT_PAREN, "(");

                List<string> arguments = new List<string>();
                while (Match(TokenType.IDENTIFIER))
                {
                    arguments.Add(GetLexeme());
                    Advance();
                    if (Match(TokenType.COMMA))
                        Advance();
                }

                Consume(TokenType.RIGHT_PAREN, ")");

                Consume(TokenType.INLINE_FUNCTION, "=>");

                if (Functions.Contains(identifier))
                {
                    throw new Exception("Functions cannot be redefined.");
                }
                Functions.Add(identifier);
                delete = identifier;

                Expression body = ParseExpression();

                Consume(TokenType.SEMICOLON, ";");

                Functions.Add(identifier, new Expression.Function(identifier, arguments, body));

                return Functions.Get(identifier);
            }

            Expression expr = ParseExpression();

            Consume(TokenType.SEMICOLON, ";");

            if (GetType() == TokenType.EOF)
                return expr;
            else
                throw new Error(ErrorType.SYNTAX_ERROR, "Invalid syntax", GetToken());
        }
        catch (Error error)
        {
            if (delete != "")
                Functions.Erase(delete);

            error.Report();
            return null!;
        }
        catch (Exception excp)
        {
            Console.WriteLine(excp.Message);
            return null!;
        }
    }

    /// <summary>
    /// Parse Expression Method for building the AST
    /// </summary>
    /// <returns>Root of the AST</returns>
    private Expression ParseExpression()
    {
        return Logical();
    }

    /// <summary>
    /// Parse Logical Expression (& |)
    /// </summary>
    /// <returns>Logical Expression</returns>
    private Expression Logical()
    {
        Expression expr = Equality();

        while (Match(TokenType.AMPERSAND, TokenType.VER_BAR))
        {
            Token token = GetToken();
            Advance();
            Expression right = Equality();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Parse Equality Expression (== !=)
    /// </summary>
    /// <returns>Equality Expression</returns>
    private Expression Equality()
    {
        Expression expr = Comparison();

        while (Match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
        {
            Token token = GetToken();
            Advance();
            Expression right = Comparison();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Comparison Expression (< <= > >=)
    /// </summary>
    /// <returns>Comparison Expression</returns>
    private Expression Comparison()
    {
        Expression expr = Concatenation();

        while (
            Match(TokenType.LESS, TokenType.LESS_EQUAL, TokenType.GREATER, TokenType.GREATER_EQUAL)
        )
        {
            Token token = GetToken();
            Advance();
            Expression right = Concatenation();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Concatenation Expression (@)
    /// </summary>
    /// <returns>Concatenation Expression</returns>
    private Expression Concatenation()
    {
        Expression expr = Term();

        while (Match(TokenType.CONCAT))
        {
            Token token = GetToken();
            Advance();
            Expression right = Term();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Term Expression (+ -)
    /// </summary>
    /// <returns>Term Expression</returns>
    private Expression Term()
    {
        Expression expr = Factor();

        while (Match(TokenType.PLUS, TokenType.MINUS))
        {
            Token token = GetToken();
            Advance();
            Expression right = Factor();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Factor Expression (* / %)
    /// </summary>
    /// <returns>Factor Expression</returns>
    private Expression Factor()
    {
        Expression expr = Power();

        while (Match(TokenType.STAR, TokenType.SLASH, TokenType.MOD))
        {
            Token token = GetToken();
            Advance();
            Expression right = Power();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    /// <summary>
    /// Power Expression (^)
    /// </summary>
    /// <returns>Power Expression</returns>
    private Expression Power()
    {
        Expression expr = Unary();

        while (Match(TokenType.CARET))
        {
            Token token = GetToken();
            Advance();
            Expression right = Unary();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }
    
    /// <summary>
    /// Unary Expression (- !)
    /// </summary>
    /// <returns>Unary Expression</returns>
    private Expression Unary()
    {
        if (Match(TokenType.MINUS, TokenType.BANG))
        {
            Token token = GetToken();
            Advance();
            Expression right = Unary();
            return new Expression.Unary(token, right);
        }

        return Statement();
    }

    /// <summary>
    /// Assign Expression (x1=val1, x2=val2, ...)
    /// </summary>
    /// <returns>Assign Expression</returns>
    /// <exception cref="Error"></exception>
    private List<Expression.Assign> Assign()
    {
        List<Expression.Assign> assigns = new List<Expression.Assign>();

        while (GetType() != TokenType.IN)
        {
            string identifier;
            if (Match(TokenType.IDENTIFIER))
            {
                identifier = GetLexeme();
                Advance();
            }
            else
                throw new Error(ErrorType.SYNTAX_ERROR, "Expect variable name", GetToken());

            Consume(TokenType.EQUAL, "=");
            Expression expr = ParseExpression();

            if (!Match(TokenType.IN))
            {
                Consume(TokenType.COMMA, ",");

                if (Match(TokenType.IN))
                    throw new Error(ErrorType.SYNTAX_ERROR, "Expect variable name", GetToken());
            }

            assigns.Add(new Expression.Assign(identifier, expr));
        }

        return assigns;
    }

    /// <summary>
    /// Statement Expression (if else let)
    /// </summary>
    /// <returns>Statement Expression</returns>
    private Expression Statement()
    {
        if (Match(TokenType.IF))
        {
            Advance();
            Expression condition = Primary();
            Expression ifBody = ParseExpression();
            Consume(TokenType.ELSE, "else");
            Expression elseBody = ParseExpression();
            return new Expression.IfStatement(condition, ifBody, elseBody);
        }

        if (Match(TokenType.LET))
        {
            Advance();
            List<Expression.Assign> assingBody = Assign();
            Consume(TokenType.IN, "in");
            Expression body = ParseExpression();
            return new Expression.LetStatement(assingBody, body);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            if (Functions.Contains(GetLexeme()))
            {
                string identifier = GetLexeme();
                Advance();
                Consume(TokenType.LEFT_PAREN, "(");

                List<Expression> arguments = new List<Expression>();
                while (GetType() != TokenType.RIGHT_PAREN)
                {
                    Expression argument = ParseExpression();

                    arguments.Add(argument);

                    if (GetType() != TokenType.RIGHT_PAREN)
                        Consume(TokenType.COMMA, ")");
                }

                Consume(TokenType.RIGHT_PAREN, ")");

                return new Expression.Call(identifier, arguments, Functions.Get(identifier));
            }

            Expression.Variable expr = new Expression.Variable(GetLexeme());
            Advance();
            return expr;
        }

        return Primary();
    }

    /// <summary>
    /// Primary Expression
    /// </summary>
    /// <returns>Primary Expression</returns>
    /// <exception cref="Error"></exception>
    private Expression Primary()
    {
        if (
            Match(
                TokenType.NUMBER,
                TokenType.STRING,
                TokenType.FALSE,
                TokenType.TRUE,
                TokenType.PI,
                TokenType.EULER
            )
        )
            return new Expression.Literal(tokens[Advance()].literal);

        if (Match(TokenType.LEFT_PAREN))
        {
            Advance();
            Expression expr = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, ")");
            return expr;
        }

        throw new Error(ErrorType.SYNTAX_ERROR, "Invalid syntax", GetToken());
    }
}