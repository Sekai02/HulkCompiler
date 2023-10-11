namespace HULKLibrary;

public enum TokenType
{
    //Tokens with one character
    LEFT_PAREN,
    RIGHT_PAREN,
    COMMA,
    MINUS,
    PLUS,
    SEMICOLON,
    SLASH,
    MOD,
    STAR,
    CONCAT,
    CARET,
    AMPERSAND,
    VER_BAR,

    //Tokens with one or two characters
    BANG,
    BANG_EQUAL,
    EQUAL,
    EQUAL_EQUAL,
    GREATER,
    GREATER_EQUAL,
    LESS,
    LESS_EQUAL,

    //Literal Tokens
    IDENTIFIER,
    STRING,
    NUMBER,

    //Keywords
    IF,
    ELSE,
    TRUE,
    FALSE,  
    FUNCTION,
    INLINE_FUNCTION,
    CLASS,
    FOR,
    WHILE,
    RETURN,
    NULL,
    PRINT,
    SIN,
    COS,
    LOG,
    EXP,
    SQRT,
    RAND,
    LET,
    IN,

    //CONSTANTS
    PI,
    EULER,

    EOF
}