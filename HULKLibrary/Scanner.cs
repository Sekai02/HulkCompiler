using System.Collections;
using System.Dynamic;
using System.Security.Principal;

namespace HULKLibrary;
public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    public static int line = 0;

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

    public Scanner(string source)
    {
        line++;
        this.source = source;
    }

    public List<Token> scanTokens()
    {
        while (!isAtEnd())
        {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private void scanToken()
    {
        char c = advance();
        switch (c)
        {
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case ',': addToken(TokenType.COMMA); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '*': addToken(TokenType.STAR); break;
            case '@': addToken(TokenType.CONCAT); break;
            case '^': addToken(TokenType.CARET); break;
            case '/': addToken(TokenType.SLASH); break;
            case '%': addToken(TokenType.MOD); break;
            case '&': addToken(TokenType.AMPERSAND); break;
            case '|': addToken(TokenType.VER_BAR); break;

            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                if (match('=')) addToken(TokenType.EQUAL_EQUAL);
                else if (match('>')) addToken(TokenType.INLINE_FUNCTION);
                else addToken(TokenType.EQUAL);
                //addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;

            case '"':
                _string();
                break;

            default:
                if (isDigit(c))
                {
                    number();
                }
                else if (isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    Error err = new Error(ErrorType.LEXICAL_ERROR, "Unexpected character.", line);
                    err.Report();
                }
                break;
        }
    }

    private void identifier()
    {
        while (isAlphaNumeric(peek())) advance();

        string text = Substring(source, start, current);
        TokenType type;
        object literal = null;
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

        if (literal == null) addToken(type);
        else addToken(type, literal);
    }

    private bool isAlpha(char c)
    {
        return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_');
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private void number()
    {
        while (isDigit(peek())) advance();

        if (peek() == '.' && isDigit(peekNext()))
        {
            advance();

            while (isDigit(peek())) advance();
        }

        addToken(TokenType.NUMBER, Double.Parse(Substring(source, start, current)));
    }

    private void _string()
    {
        while (peek() != '"' && !isAtEnd())
        {
            if (peek() == '\n') line++;
            advance();
        }

        if (isAtEnd())
        {
            Error err = new Error(ErrorType.LEXICAL_ERROR, "Unterminated string.", line);
            err.Report();
            return;
        }

        advance();

        string value = Substring(source, start + 1, current - 1);
        addToken(TokenType.STRING, value);
    }

    private bool match(char expected)
    {
        if (isAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char peek()
    {
        if (isAtEnd()) return '\0';
        return source[current];
    }

    private char peekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool isAtEnd()
    {
        return current >= source.Length;
    }

    private char advance()
    {
        return source[current++];
    }

    private string Substring(string s, int begIdx, int endIdx)
    {
        return s.Substring(begIdx, endIdx - begIdx);
    }

    private void addToken(TokenType type)
    {
        addToken(type, null);
    }

    private void addToken(TokenType type, object literal)
    {
        //string text = source.Substring(start, current);
        string text = Substring(source, start, current);
        tokens.Add(new Token(type, text, literal, line));
    }
}