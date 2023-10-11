namespace HULKLibrary;

public class Parser
{
    private readonly List<Token> tokens;
    int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Token getToken()
    {
        return tokens[current];
    }

    private new TokenType getType()
    {
        return tokens[current].type;
    }

    private string getLexeme()
    {
        return tokens[current].lexeme;
    }

    private bool match(params TokenType[] tokentypes)
    {
        if (getType() == TokenType.EOF)
            return false;

        foreach (TokenType type in tokentypes)
            if (getType() == type)
                return true;

        return false;
    }

    private int advance()
    {
        if (getType() != TokenType.EOF)
            current++;

        return current - 1;
    }

    private int consume(TokenType type, string lexeme)
    {
        if (match(type))
            return advance();

        throw new Error(ErrorType.SYNTAX_ERROR, "Expect '" + lexeme, getToken());
    }

    public Expression parse()
    {
        string delete = "";
        try
        {
            if (match(TokenType.FUNCTION))
            {
                advance();
                string identifier = "";

                if (match(TokenType.IDENTIFIER))
                    identifier = getLexeme();

                consume(TokenType.IDENTIFIER, "a name");

                consume(TokenType.LEFT_PAREN, "(");

                List<string> arguments = new List<string>();
                while (match(TokenType.IDENTIFIER))
                {
                    arguments.Add(getLexeme());
                    advance();
                    if (match(TokenType.COMMA))
                        advance();
                }

                consume(TokenType.RIGHT_PAREN, ")");

                consume(TokenType.INLINE_FUNCTION, "=>");

                if (Functions.contains(identifier))
                {
                    throw new Exception("Functions cannot be redefined.");
                }
                Functions.add(identifier);
                delete = identifier;

                Expression body = expression();

                consume(TokenType.SEMICOLON, ";");

                Functions.add(identifier, new Expression.Function(identifier, arguments, body));

                return Functions.get(identifier);
            }

            Expression expr = expression();

            consume(TokenType.SEMICOLON, ";");

            if (getType() == TokenType.EOF)
                return expr;
            else
                throw new Error(ErrorType.SYNTAX_ERROR, "Invalid syntax", getToken());
        }
        catch (Error error)
        {
            if (delete != "")
                Functions.erase(delete);

            error.Report();
            return null!;
        }
    }

    private Expression expression()
    {
        return logical();
    }

    private Expression logical()
    {
        Expression expr = equality();

        while (match(TokenType.AMPERSAND, TokenType.VER_BAR))
        {
            Token token = getToken();
            advance();
            Expression right = equality();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression equality()
    {
        Expression expr = comparison();

        while (match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
        {
            Token token = getToken();
            advance();
            Expression right = comparison();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression comparison()
    {
        Expression expr = concatenation();

        while (
            match(TokenType.LESS, TokenType.LESS_EQUAL, TokenType.GREATER, TokenType.GREATER_EQUAL)
        )
        {
            Token token = getToken();
            advance();
            Expression right = concatenation();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression concatenation()
    {
        Expression expr = term();

        while (match(TokenType.CONCAT))
        {
            Token token = getToken();
            advance();
            Expression right = term();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression term()
    {
        Expression expr = factor();

        while (match(TokenType.PLUS, TokenType.MINUS))
        {
            Token token = getToken();
            advance();
            Expression right = factor();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression factor()
    {
        Expression expr = power();

        while (match(TokenType.STAR, TokenType.SLASH, TokenType.MOD))
        {
            Token token = getToken();
            advance();
            Expression right = power();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression power()
    {
        Expression expr = unary();

        while (match(TokenType.CARET))
        {
            Token token = getToken();
            advance();
            Expression right = unary();
            expr = new Expression.Binary(expr, token, right);
        }

        return expr;
    }

    private Expression unary()
    {
        if (match(TokenType.MINUS, TokenType.BANG))
        {
            Token token = getToken();
            advance();
            Expression right = unary();
            return new Expression.Unary(token, right);
        }

        return statement();
    }

    private List<Expression.Assign> assing()
    {
        List<Expression.Assign> assings = new List<Expression.Assign>();

        while (getType() != TokenType.IN)
        {
            string identifier;
            if (match(TokenType.IDENTIFIER))
            {
                identifier = getLexeme();
                advance();
            }
            else
                throw new Error(ErrorType.SYNTAX_ERROR, "Expect variable name", getToken());

            consume(TokenType.EQUAL, "=");
            Expression expr = expression();

            if (!match(TokenType.IN))
            {
                consume(TokenType.COMMA, ",");

                if (match(TokenType.IN))
                    throw new Error(ErrorType.SYNTAX_ERROR, "Expect variable name", getToken());
            }

            assings.Add(new Expression.Assign(identifier, expr));
        }

        return assings;
    }

    private Expression statement()
    {
        if (match(TokenType.IF))
        {
            advance();
            Expression condition = primary();
            Expression ifBody = expression();
            consume(TokenType.ELSE, "else");
            Expression elseBody = expression();
            return new Expression.IfStatement(condition, ifBody, elseBody);
        }

        if (match(TokenType.LET))
        {
            advance();
            List<Expression.Assign> assingBody = assing();
            consume(TokenType.IN, "in");
            Expression body = expression();
            return new Expression.LetStatement(assingBody, body);
        }

        if (match(TokenType.IDENTIFIER))
        {
            if (Functions.contains(getLexeme()))
            {
                string identifier = getLexeme();
                advance();
                consume(TokenType.LEFT_PAREN, "(");

                List<Expression> arguments = new List<Expression>();
                while (getType() != TokenType.RIGHT_PAREN)
                {
                    Expression argument = expression();

                    arguments.Add(argument);

                    if (getType() != TokenType.RIGHT_PAREN)
                        consume(TokenType.COMMA, ")");
                }

                consume(TokenType.RIGHT_PAREN, ")");

                return new Expression.Call(identifier, arguments, Functions.get(identifier));
            }

            Expression.Variable expr = new Expression.Variable(getLexeme());
            advance();
            return expr;
        }

        return primary();
    }

    private Expression primary()
    {
        if (
            match(
                TokenType.NUMBER,
                TokenType.STRING,
                TokenType.FALSE,
                TokenType.TRUE,
                TokenType.PI,
                TokenType.EULER
            )
        )
            return new Expression.Literal(tokens[advance()].literal);

        if (match(TokenType.LEFT_PAREN))
        {
            advance();
            Expression expr = expression();
            consume(TokenType.RIGHT_PAREN, ")");
            return expr;
        }

        throw new Error(ErrorType.SYNTAX_ERROR, "Invalid syntax", getToken());
    }
}