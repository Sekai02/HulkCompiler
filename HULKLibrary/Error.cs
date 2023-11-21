namespace HULKLibrary;

/// <summary>
/// Enum for representing types of error
/// </summary>
public enum ErrorType
{
    LEXICAL_ERROR,
    SYNTAX_ERROR,
    SEMANTIC_ERROR
}

/// <summary>
/// Class for error handling
/// </summary>
public class Error : Exception
{
    /// <summary>
    /// Error class constructor overload for message
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    public Error(ErrorType type, string message)
    {
        this.type = type;
        this.message = message;
        this.token = null!;
        hadError = true;
    }

    /// <summary>
    /// Error clas constructor overload for token
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    public Error(ErrorType type, string message, Token token)
    {
        this.type = type;
        this.message = message;
        this.token = token;

        hadError = true;
    }

    /// <summary>
    /// Error class constructor overload for line
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="line"></param>
    public Error(ErrorType type, string message, int line)
    {
        this.type = type;
        this.message = message;
        this.line = line;
        this.token = null!;

        hadError = true;
    }
    
    /// <summary>
    /// Method for reporting errors
    /// </summary>
    public void Report()
    {
        switch (type)
        {
            case ErrorType.LEXICAL_ERROR:
                LexicalError();
                break;
            case ErrorType.SYNTAX_ERROR:
                SyntaxError();
                break;
            case ErrorType.SEMANTIC_ERROR:
                SemanticError();
                break;
        }
    }

    /// <summary>
    /// Print lexical error
    /// </summary>
    private void LexicalError()
    {
        if (token == null)
            Console.WriteLine("{0}: {1} [line {2}]", type, message, line);
        else
            Console.WriteLine("{0}: {1} at {2} [line {3}]", type, message, token.lexeme, token.line);
    }

    /// <summary>
    /// Print syntax error
    /// </summary>
    private void SyntaxError()
    {
        if (token == null)
            Console.WriteLine("{0}: {1} [line {2}]", type, message, line);
        else
            Console.WriteLine("{0}: {1} at {2} [line {3}]", type, message, token.lexeme, token.line);
    }

    /// <summary>
    /// Print semantic error
    /// </summary>
    private void SemanticError()
    {
        if (token == null)
            Console.WriteLine("{0}: {1} [line {2}]", type, message, line);
        else
            Console.WriteLine("{0}: {1} at {2} [line {3}]", type, message, token.lexeme, token.line);
    }

    /// <summary>
    /// Indicates if the program had errors
    /// </summary>
    public static bool hadError = false;
    /// <summary>
    /// Error type of current error
    /// </summary>
    readonly ErrorType type;
    /// <summary>
    /// Message of current error
    /// </summary>
    readonly string message;
    /// <summary>
    /// Token type present in error
    /// </summary>
    readonly Token token;
    /// <summary>
    /// Error line
    /// </summary>
    readonly int line;
}