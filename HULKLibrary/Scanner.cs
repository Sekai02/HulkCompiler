using System.Collections;
using System.Dynamic;
using System.Security.Principal;

namespace HULKLibrary;

/// <summary>
/// Scanner Class (Proccesses the string creating the tokens)
/// </summary>
public class Scanner
{
    /// <summary>
    /// String to proccess
    /// </summary>
    private readonly string source;
    /// <summary>
    /// List of tokens to be build from source
    /// </summary>
    private readonly List<Token> tokens = new List<Token>();
    /// <summary>
    /// start of the current token
    /// </summary>
    private int start = 0;
    /// <summary>
    /// current position in source
    /// </summary>
    private int current = 0;
    /// <summary>
    /// current line in code
    /// </summary>
    public static int line = 0;

    /// <summary>
    /// A Dictionary containing all keywords of the language
    /// </summary>
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(){
        {"if",TokenType.IF},
        {"else",TokenType.ELSE},
        {"true",TokenType.TRUE},
        {"false",TokenType.FALSE},
        {"let",TokenType.LET},
        {"in",TokenType.IN},
        {"function",TokenType.FUNCTION},
        {"null",TokenType.NULL},
        {"PI",TokenType.PI},
        {"E",TokenType.EULER}
    };

    /// <summary>
    /// Scanner constructor
    /// </summary>
    /// <param name="source">string code to be Scanned(Tokenized)</param>
    public Scanner(string source)
    {
        line++;
        this.source = source;
    }

    /// <summary>
    /// Method to Scan all Tokens and build the List to be proccessed by parser
    /// </summary>
    /// <returns>A List of all the tokens</returns>
    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null!, line));
        return tokens;
    }

    /// <summary>
    /// Method that proccesses all the characters and see wich case matches to build the right token
    /// </summary>
    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '@': AddToken(TokenType.CONCAT); break;
            case '^': AddToken(TokenType.CARET); break;
            case '/': AddToken(TokenType.SLASH); break;
            case '%': AddToken(TokenType.MOD); break;
            case '&': AddToken(TokenType.AMPERSAND); break;
            case '|': AddToken(TokenType.VER_BAR); break;

            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                if (Match('=')) AddToken(TokenType.EQUAL_EQUAL);
                else if (Match('>')) AddToken(TokenType.INLINE_FUNCTION);
                else AddToken(TokenType.EQUAL);
                //AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;

            case '"':
                String();
                break;

            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Error err = new Error(ErrorType.LEXICAL_ERROR, "Unexpected character.", line);
                    err.Report();
                }
                break;
        }
    }

    /// <summary>
    /// Tokenize an identifier
    /// </summary>
    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = Substring(source, start, current);
        TokenType type;
        object literal = null!;
        if (keywords.TryGetValue(text, out _))
        {
            type = keywords[text];

            switch (type)
            {
                case TokenType.FALSE:
                    literal = false;
                    break;
                case TokenType.TRUE:
                    literal = true;
                    break;
                case TokenType.EULER:
                    literal = Math.E;
                    break;
                case TokenType.PI:
                    literal = Math.PI;
                    break;
                default:
                    break;
            }
        }
        else
        {
            type = TokenType.IDENTIFIER;
        }

        if (literal == null) AddToken(type);
        else AddToken(type, literal);
    }

    /// <summary>
    /// Utility method to check if character is alphabetic
    /// </summary>
    /// <param name="c">the character we want to check</param>
    /// <returns>true if is alphabetic, false otherwise</returns>
    private bool IsAlpha(char c)
    {
        return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_');
    }

    /// <summary>
    /// Checks if a character is alphabetic or numeric
    /// </summary>
    /// <param name="c">the character we want to check</param>
    /// <returns>true if is alphanumeric, false otherwise</returns>
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    /// <summary>
    /// Tokenize a number
    /// </summary>
    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.NUMBER, Double.Parse(Substring(source, start, current)));
    }

    /// <summary>
    /// Tokenize a string
    /// </summary>
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Error err = new Error(ErrorType.LEXICAL_ERROR, "Unterminated string.", line);
            err.Report();
            return;
        }

        Advance();

        string value = Substring(source, start + 1, current - 1);
        AddToken(TokenType.STRING, value);
    }

    /// <summary>
    /// Utility method to check if the current character in source matches the expected one
    /// </summary>
    /// <param name="expected">the expected character</param>
    /// <returns>true if matches, false otherwise</returns>
    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    /// <summary>
    /// Returns the current character
    /// </summary>
    /// <returns>the current character in source</returns>
    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    /// <summary>
    /// Return the next character
    /// </summary>
    /// <returns>the next character in source</returns>
    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    /// <summary>
    /// Utility method to check if the given character is a digit
    /// </summary>
    /// <param name="c">the character we want to check</param>
    /// <returns>true if it is digit, false otherwise</returns>
    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    /// <summary>
    /// Checks if we are at the end of the code
    /// </summary>
    /// <returns>True if we are at end, false otherwise</returns>
    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    /// <summary>
    /// Returns the current character and advances to the next
    /// </summary>
    /// <returns>the current character in source</returns>
    private char Advance()
    {
        return source[current++];
    }

    /// <summary>
    /// Takes the substring of s in the range [begIdx,endIdx]
    /// </summary>
    /// <param name="s">string to extract the substring</param>
    /// <param name="begIdx">range start</param>
    /// <param name="endIdx">range end</param>
    /// <returns>the substring between [begIdx,endIdx]</returns>
    private string Substring(string s, int begIdx, int endIdx)
    {
        return s.Substring(begIdx, endIdx - begIdx);
    }

    /// <summary>
    /// Add a token to the List of tokens to parse
    /// </summary>
    /// <param name="type">type of token</param>
    private void AddToken(TokenType type)
    {
        AddToken(type, null!);
    }

    /// <summary>
    /// Add a token with a given literal value
    /// </summary>
    /// <param name="type">type of token</param>
    /// <param name="literal">value literal</param>
    private void AddToken(TokenType type, object literal)
    {
        //string text = source.Substring(start, current);
        string text = Substring(source, start, current);
        tokens.Add(new Token(type, text, literal, line));
    }
}