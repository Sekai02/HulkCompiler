namespace HULKLibrary;

public class Token
{
    /// <summary>
    /// Token Type
    /// </summary>
    public readonly TokenType type;
    /// <summary>
    /// Name of the Token
    /// </summary>
    public readonly string lexeme;
    /// <summary>
    /// Value literal
    /// </summary>
    public readonly object literal;
    /// <summary>
    /// Line in wich the token is present
    /// </summary>
    public readonly int line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public override string ToString()
    {
        return type + " " + lexeme + " " + literal;
    }
}