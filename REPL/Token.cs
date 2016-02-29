using System;
using System.Text;
namespace REPL
{
    enum TokenType
    {
        ILLEGAL,
        EOF,
        LPAREN,
        RPAREN,
        LBRACK,
        RBRACK,
        LBRACE,
        RBRACE,
        COLON,
        SEMICOLON,
        PERIOD,
        CONDITIONAL,
        ARROW,
        ASSIGN,
        ASSIGN_ADD,
        ASSIGN_SUB,
        ASSIGN_MUL,
        ASSIGN_DIV,
        ASSIGN_MOD,
        COMMA,
        OR,
        AND,
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        EQ,
        NE,
        LT,
        GT,
        LTE,
        GTE,
        NOT,
        BREAK,
        CONTINUE,
        ELSE,
        FOR,
        FUNCTION,
        IF,
        NEW,
        RETURN,
        VAR,
        WHILE,
        NULL_LITERAL,
        TRUE_LITERAL,
        FALSE_LITERAL,
        NUMBER,
        STRING,
        IDENTIFIER,
        THIS
    }



    static class TokenHelper
    {

        public static string Name(TokenType t)
        {
            return t.ToString();
        }

        public static string Value(TokenType t)
        {
            switch (t)
            {
                case TokenType.ILLEGAL: return "ILLEGAL";
                case TokenType.EOF: return "EOS";
                case TokenType.LPAREN: return "(";
                case TokenType.RPAREN: return ")";
                case TokenType.LBRACK: return "[";
                case TokenType.RBRACK: return "]";
                case TokenType.LBRACE: return "{";
                case TokenType.RBRACE: return "}";
                case TokenType.COLON: return ":";
                case TokenType.SEMICOLON: return ";";
                case TokenType.PERIOD: return ".";
                case TokenType.CONDITIONAL: return "?";
                case TokenType.ARROW: return "=>";
                case TokenType.ASSIGN: return "=";
                case TokenType.ASSIGN_ADD: return "+=";
                case TokenType.ASSIGN_SUB: return "-=";
                case TokenType.ASSIGN_MUL: return "*=";
                case TokenType.ASSIGN_DIV: return "/=";
                case TokenType.ASSIGN_MOD: return "%=";
                case TokenType.COMMA: return ",";
                case TokenType.OR: return "|";
                case TokenType.AND: return "&";
                case TokenType.ADD: return "+";
                case TokenType.SUB: return "-";
                case TokenType.MUL: return "*";
                case TokenType.DIV: return "/";
                case TokenType.MOD: return "%";
                case TokenType.EQ: return "==";
                case TokenType.NE: return "!=";
                case TokenType.LT: return "<";
                case TokenType.GT: return ">";
                case TokenType.LTE: return "<=";
                case TokenType.GTE: return ">=";
                case TokenType.NOT: return "!";
                case TokenType.BREAK: return "break";
                case TokenType.CONTINUE: return "continue";
                case TokenType.ELSE: return "else";
                case TokenType.FOR: return "for";
                case TokenType.FUNCTION: return "function";
                case TokenType.IF: return "if";
                case TokenType.NEW: return "new";
                case TokenType.RETURN: return "return";
                case TokenType.VAR: return "var";
                case TokenType.WHILE: return "while";
                case TokenType.NULL_LITERAL: return "null";
                case TokenType.TRUE_LITERAL: return "true";
                case TokenType.FALSE_LITERAL: return "false";
                case TokenType.NUMBER: return "NUMBER";
                case TokenType.STRING: return "STRING";
                case TokenType.IDENTIFIER: return "IDENTIFIER";
                case TokenType.THIS: return "this";
                default: return "ILLEGAL";
            }
        }

        public static int Precedence(TokenType t)
        {
            switch (t)
            {
                case TokenType.ILLEGAL: return 0;
                case TokenType.EOF: return 0;
                case TokenType.LPAREN: return 14;
                case TokenType.RPAREN: return 0;
                case TokenType.LBRACK: return 14;
                case TokenType.RBRACK: return 0;
                case TokenType.LBRACE: return 0;
                case TokenType.RBRACE: return 0;
                case TokenType.COLON: return 0;
                case TokenType.SEMICOLON: return 0;
                case TokenType.PERIOD: return 14;
                case TokenType.CONDITIONAL: return 3;
                case TokenType.ARROW: return 3;
                case TokenType.ASSIGN: return 2;
                case TokenType.ASSIGN_ADD: return 2;
                case TokenType.ASSIGN_SUB: return 2;
                case TokenType.ASSIGN_MUL: return 2;
                case TokenType.ASSIGN_DIV: return 2;
                case TokenType.ASSIGN_MOD: return 2;
                case TokenType.COMMA: return 1;
                case TokenType.OR: return 4;
                case TokenType.AND: return 5;
                case TokenType.ADD: return 12;
                case TokenType.SUB: return 12;
                case TokenType.MUL: return 13;
                case TokenType.DIV: return 13;
                case TokenType.MOD: return 13;
                case TokenType.EQ: return 9;
                case TokenType.NE: return 9;
                case TokenType.LT: return 10;
                case TokenType.GT: return 10;
                case TokenType.LTE: return 10;
                case TokenType.GTE: return 10;
                case TokenType.NOT: return 0;
                case TokenType.BREAK: return 0;
                case TokenType.CONTINUE: return 0;
                case TokenType.ELSE: return 0;
                case TokenType.FOR: return 0;
                case TokenType.FUNCTION: return 0;
                case TokenType.IF: return 0;
                case TokenType.NEW: return 0;
                case TokenType.RETURN: return 0;
                case TokenType.VAR: return 0;
                case TokenType.WHILE: return 0;
                case TokenType.NULL_LITERAL: return 0;
                case TokenType.TRUE_LITERAL: return 0;
                case TokenType.FALSE_LITERAL: return 0;
                case TokenType.NUMBER: return 0;
                case TokenType.STRING: return 0;
                case TokenType.IDENTIFIER: return 0;
                case TokenType.THIS: return 0;
                default: return 0;
            }
        }

        public static TokenType NameToToken(string name)
        {
            switch (name)
            {
                case "break": return TokenType.BREAK;
                case "continue": return TokenType.CONTINUE;
                case "else": return TokenType.ELSE;
                case "for": return TokenType.FOR;
                case "fn": return TokenType.FUNCTION;
                case "if": return TokenType.IF;
                case "new": return TokenType.NEW;
                case "return": return TokenType.RETURN;
                case "var": return TokenType.VAR;
                case "while": return TokenType.WHILE;
                case "null": return TokenType.NULL_LITERAL;
                case "true": return TokenType.TRUE_LITERAL;
                case "false": return TokenType.FALSE_LITERAL;
                case "this": return TokenType.THIS;
                default: return TokenType.IDENTIFIER;
            }
        }
    }
}
