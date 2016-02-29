using System;
using System.IO;
using System.Collections.Generic;
using REPL.AST;
using System.Text;


namespace REPL
{
    partial class Parser
    {
        
        #region MiscFunctions
        public Parser(TextReader input)
        {
            Input = input;
            NextChar();
            //this doesn't matter as long as it isnt EOF or ILLEGAL;
            Token = TokenType.ADD;
            NextToken();
        }

        private void Error(string format, params object[] data)
        {
            throw new SyntaxError(String.Format(format, data), Line);
        }

        private void Error(string code)
        {
            throw new SyntaxError(code, Line);
        }

        private void UnexpectedToken()
        {
            Error("Unexpected Token {0}", TokenHelper.Name(Token));
        }

        private T NewNode<T>() where T : AstNode, new()
        {
            return new T {
                Line = Line,
                Token = Token
            };
        }

        private void ExpectToken(TokenType tk)
        {
            if (Token != tk) {
                Error("Expected token {0} instead saw {1}",
                    TokenHelper.Name(tk),
                    TokenHelper.Name(Token));
            }
        }
        #endregion

        #region TokenStream
        private TokenType Token;
        private int Line = 1;
        private string StrVal;
        private double? NumVal;
        private TextReader Input;
        private int TokenPrecedence
        {
            get { return TokenHelper.Precedence(Token); }
        }


        private void NextToken()
        {
            if (Token == TokenType.ILLEGAL || Token == TokenType.EOF)
                return;

            StrVal = null;
            NumVal = null;

        start:
            if (c == '/' && PeekChar() == '/') {
                SkipComment();
                goto start;
            }
            switch (CharClass) {
                case CharClasses.WHITESPACE:
                    NextChar();
                    goto start;
                case CharClasses.NUMBER:
                    ScanNumber();
                    return;
                case CharClasses.NAME:
                    ScanName();
                    return;
                case CharClasses.OPPERATOR:
                    ScanOpperator();
                    return;
                case CharClasses.EOF:
                    Token = TokenType.EOF;
                    return;
                case CharClasses.ILLEGAL:
                    Token = TokenType.ILLEGAL;
                    return;
                case CharClasses.STRING:
                    Token = TokenType.STRING;
                    ScanString();
                    return;
            }
        }

        private void ScanString()
        {
            NextChar();
            StringBuilder str = new StringBuilder();
            while (c != '"') {
                if (c == '\\') {
                    NextChar();
                    switch (c) {
                        case '"': c = '"'; break;
                        case '\\': c = '\\'; break;
                        case 't': c = '\t'; break;
                        case 'b': c = '\b'; break;
                        case 'f': c = '\f'; break;
                        case 'n': c = '\n'; break;
                        case 'r': c = '\r'; break;
                        default:
                            Error("Unknown Escape Sequence");
                            return;
                    }
                }
                str.Append((char)c);
                NextChar();
            }
            StrVal = str.ToString();
            NextChar();
        }

        private void ScanOpperator()
        {
            char sw = (char)c;
            NextChar();
            switch (sw) {
                case '(':
                    Token = TokenType.LPAREN;
                    break;
                case ')':
                    Token = TokenType.RPAREN;
                    break;
                case '[':
                    Token = TokenType.LBRACK;
                    break;
                case ']':
                    Token = TokenType.RBRACK;
                    break;
                case '{':
                    Token = TokenType.LBRACE;
                    break;
                case '}':
                    Token = TokenType.RBRACE;
                    break;
                case ':':
                    Token = TokenType.COLON;
                    break;
                case ';':
                    Token = TokenType.SEMICOLON;
                    break;
                case '=':
                    switch (c) {
                        case '>':
                            NextChar();
                            Token = TokenType.ARROW;
                            break;
                        case '=':
                            NextChar();
                            Token = TokenType.EQ;
                            break;
                        default:
                            Token = TokenType.ASSIGN;
                            break;
                    }
                    break;
                case '.':
                    Token = TokenType.PERIOD;
                    break;
                case '?':
                    Token = TokenType.CONDITIONAL;
                    break;
                case '+':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.ASSIGN_ADD;
                        break;
                    }
                    Token = TokenType.ADD;
                    break;
                case '-':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.ASSIGN_SUB;
                        break;
                    }
                    Token = TokenType.SUB;
                    break;
                case '*':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.ASSIGN_MUL;
                        break;
                    }
                    Token = TokenType.MUL;
                    break;
                case '/':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.ASSIGN_DIV;
                        break;
                    }
                    Token = TokenType.DIV;
                    break;
                case '%':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.ASSIGN_MOD;
                        break;
                    }
                    Token = TokenType.MOD;
                    break;
                case ',':
                    Token = TokenType.COMMA;
                    break;
                case '>':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.GTE;
                        break;
                    }
                    Token = TokenType.GT;
                    break;
                case '<':
                    if (c == '=') {
                        NextChar();
                        Token = TokenType.LTE;
                        break;
                    }
                    Token = TokenType.LT;
                    break;
                case '!':
                    if (c == '=') {
                        NextToken();
                        Token = TokenType.NE;
                        break;
                    }
                    Token = TokenType.NOT;
                    break;
                case '&':
                    Token = TokenType.AND;
                    break;
                case '|':
                    Token = TokenType.OR;
                    break;
                default: break;
                //unreachable
            }
        }

        private void ScanName()
        {
            StringBuilder Name = new StringBuilder();
            do {
                Name.Append((char)c);
                NextChar();
            } while (CharClass == CharClasses.NUMBER || CharClass == CharClasses.NAME);
            StrVal = Name.ToString();
            Token = TokenHelper.NameToToken(StrVal);
        }

        private void ScanNumber()
        {
            StringBuilder Number = new StringBuilder();
            do {
                Number.Append((char)c);
                NextChar();
            } while (CharClass == CharClasses.NUMBER);
            if (c == '.') {
                NextChar();
                if (CharClass != CharClasses.NUMBER)
                    Error("Malformed number literal");
                do {
                    Number.Append((char)c);
                    NextChar();
                } while (CharClass == CharClasses.NUMBER);
            }
            if (c == 'e' || c == 'E') {
                NextChar();
                if (c == '+' || c == '-') {
                    Number.Append((char)c);
                    NextChar();
                }
                if (CharClass != CharClasses.NUMBER) {
                    Error("Malformed number literal");
                }
                do {
                    Number.Append((char)c);
                    NextChar();
                } while (CharClass == CharClasses.NUMBER);
            }
            Token = TokenType.NUMBER;
            NumVal = double.Parse(Number.ToString());
        }

        private void SkipComment()
        {
            do NextChar();
            while (c != '\n' && CharClass != CharClasses.EOF);
        }

        private int PeekChar()
        {
            return Input.Peek();
        }

        private void NextChar()
        {
            if (CharClass == CharClasses.EOF)
                return;

            c = Input.Read();

            if (c == -1) {
                CharClass = CharClasses.EOF;
                return;
            }

            if (c == '\n')
                Line++;

            if (c > CharClassTable.Length) {
                CharClass = CharClasses.ILLEGAL;
                return;
            }

            CharClass = CharClassTable[c];
        }

        private enum CharClasses
        {
            ILLEGAL,
            OPPERATOR,
            STRING,
            NUMBER,
            NAME,
            WHITESPACE,
            EOF
        }

        private static readonly CharClasses[] CharClassTable = new CharClasses[] {
			#region ClassMap
			CharClasses.ILLEGAL,    // 0x0
			CharClasses.ILLEGAL,    // 0x1
			CharClasses.ILLEGAL,    // 0x2
			CharClasses.ILLEGAL,    // 0x3
			CharClasses.ILLEGAL,    // 0x4
			CharClasses.ILLEGAL,    // 0x5
			CharClasses.ILLEGAL,    // 0x6
			CharClasses.ILLEGAL,    // 0x7
			CharClasses.ILLEGAL,    // 0x8
			CharClasses.WHITESPACE, // '\t'
			CharClasses.WHITESPACE, // '\n'
			CharClasses.ILLEGAL,    // 0xb
			CharClasses.ILLEGAL,    // 0xc
			CharClasses.WHITESPACE, // '\r'
			CharClasses.ILLEGAL,    // 0xe
			CharClasses.ILLEGAL,    // 0xf
			CharClasses.ILLEGAL,    // 0x10
			CharClasses.ILLEGAL,    // 0x11
			CharClasses.ILLEGAL,    // 0x12
			CharClasses.ILLEGAL,    // 0x13
			CharClasses.ILLEGAL,    // 0x14
			CharClasses.ILLEGAL,    // 0x15
			CharClasses.ILLEGAL,    // 0x16
			CharClasses.ILLEGAL,    // 0x17
			CharClasses.ILLEGAL,    // 0x18
			CharClasses.ILLEGAL,    // 0x19
			CharClasses.ILLEGAL,    // 0x1a
			CharClasses.ILLEGAL,    // 0x1b
			CharClasses.ILLEGAL,    // 0x1c
			CharClasses.ILLEGAL,    // 0x1d
			CharClasses.ILLEGAL,    // 0x1e
			CharClasses.ILLEGAL,    // 0x1f
			CharClasses.WHITESPACE, // ' '
			CharClasses.OPPERATOR,  // 0x21
			CharClasses.STRING,     // '\"'
			CharClasses.ILLEGAL,    // 0x23
			CharClasses.NAME,       // '$'
			CharClasses.OPPERATOR,  // '%'
			CharClasses.OPPERATOR,  // '&'
			CharClasses.ILLEGAL,    // 0x27
			CharClasses.OPPERATOR,  // '('
			CharClasses.OPPERATOR,  // ')'
			CharClasses.OPPERATOR,  // '*'
			CharClasses.OPPERATOR,  // '+'
			CharClasses.OPPERATOR,  // ','
			CharClasses.OPPERATOR,  // '-'
			CharClasses.OPPERATOR,  // '.'
			CharClasses.OPPERATOR,  // '/'
			CharClasses.NUMBER,     // '0'
			CharClasses.NUMBER,     // '1'
			CharClasses.NUMBER,     // '2'
			CharClasses.NUMBER,     // '3'
			CharClasses.NUMBER,     // '4'
			CharClasses.NUMBER,     // '5'
			CharClasses.NUMBER,     // '6'
			CharClasses.NUMBER,     // '7'
			CharClasses.NUMBER,     // '8'
			CharClasses.NUMBER,     // '9'
			CharClasses.OPPERATOR,  // ':'
			CharClasses.OPPERATOR,  // ';'
			CharClasses.OPPERATOR,  // '<'
			CharClasses.OPPERATOR,  // '='
			CharClasses.OPPERATOR,  // '>'
			CharClasses.OPPERATOR,  // '?'
			CharClasses.ILLEGAL,    // 0x40
			CharClasses.NAME,       // 'A'
			CharClasses.NAME,       // 'B'
			CharClasses.NAME,       // 'C'
			CharClasses.NAME,       // 'D'
			CharClasses.NAME,       // 'E'
			CharClasses.NAME,       // 'F'
			CharClasses.NAME,       // 'G'
			CharClasses.NAME,       // 'H'
			CharClasses.NAME,       // 'I'
			CharClasses.NAME,       // 'J'
			CharClasses.NAME,       // 'K'
			CharClasses.NAME,       // 'L'
			CharClasses.NAME,       // 'M'
			CharClasses.NAME,       // 'N'
			CharClasses.NAME,       // 'O'
			CharClasses.NAME,       // 'P'
			CharClasses.NAME,       // 'Q'
			CharClasses.NAME,       // 'R'
			CharClasses.NAME,       // 'S'
			CharClasses.NAME,       // 'T'
			CharClasses.NAME,       // 'U'
			CharClasses.NAME,       // 'V'
			CharClasses.NAME,       // 'W'
			CharClasses.NAME,       // 'X'
			CharClasses.NAME,       // 'Y'
			CharClasses.NAME,       // 'Z'
			CharClasses.OPPERATOR,  // '['
			CharClasses.ILLEGAL,    // 0x5c
			CharClasses.OPPERATOR,  // ']'
			CharClasses.ILLEGAL,    // 0x5e
			CharClasses.NAME,       // '_'
			CharClasses.ILLEGAL,    // 0x60
			CharClasses.NAME,       // 'a'
			CharClasses.NAME,       // 'b'
			CharClasses.NAME,       // 'c'
			CharClasses.NAME,       // 'd'
			CharClasses.NAME,       // 'e'
			CharClasses.NAME,       // 'f'
			CharClasses.NAME,       // 'g'
			CharClasses.NAME,       // 'h'
			CharClasses.NAME,       // 'i'
			CharClasses.NAME,       // 'j'
			CharClasses.NAME,       // 'k'
			CharClasses.NAME,       // 'l'
			CharClasses.NAME,       // 'm'
			CharClasses.NAME,       // 'n'
			CharClasses.NAME,       // 'o'
			CharClasses.NAME,       // 'p'
			CharClasses.NAME,       // 'q'
			CharClasses.NAME,       // 'r'
			CharClasses.NAME,       // 's'
			CharClasses.NAME,       // 't'
			CharClasses.NAME,       // 'u'
			CharClasses.NAME,       // 'v'
			CharClasses.NAME,       // 'w'
			CharClasses.NAME,       // 'x'
			CharClasses.NAME,       // 'y'
			CharClasses.NAME,       // 'z'
			CharClasses.OPPERATOR,  // '{'
			CharClasses.OPPERATOR,  // '|'
			CharClasses.OPPERATOR,  // '}'
			CharClasses.ILLEGAL,    // 0x7e
			CharClasses.ILLEGAL     // 0x7f
			#endregion
		};

        private CharClasses CharClass;
        private int c;

        #endregion

        #region Expressions
        private StringLiteral ParseStringLiteral()
        {
            StringLiteral ret = NewNode<StringLiteral>();
            ret.Value = StrVal;
            NextToken();
            return ret;
        }

        private NumberLiteralNode ParseNumberLiteral()
        {
            NumberLiteralNode ret = NewNode<NumberLiteralNode>();
            ret.Value = (double)NumVal;
            NextToken();
            return ret;
        }

        private ExpressionNode ParseParanthetical()
        {
            NextToken();
            if (Token == TokenType.RPAREN) {
                NextToken();
                ExpectToken(TokenType.ARROW);
                return null;
            }
            ExpressionNode ret = ParseExpression();
            ExpectToken(TokenType.RPAREN);
            NextToken();
            return ret;
        }

        private NewExpression ParseNewExpression()
        {
            NewExpression ret = NewNode<NewExpression>();
            NextToken();
            ret.Base = ParseExpression(13);
            if (Token == TokenType.LBRACE) {
                ret.Extend = ParseObjectLiteral();
            };
            return ret;
        }

        private UnaryExpressionNode ParseUnaryExpression()
        {
            UnaryExpressionNode ret = NewNode<UnaryExpressionNode>();
            NextToken();
            ret.Value = ParseExpression(13);
            return ret;
        }

        private VariableExpression ParseVariableExpression()
        {
            VariableExpression ret = NewNode<VariableExpression>();
            ret.Value = StrVal;
            NextToken();
            return ret;
        }

        private ExpressionNode ParseArrayLiteral()
        {
            ArrayLiteral ret = NewNode<ArrayLiteral>();
            NextToken();
            bool first = true;
            while (Token != TokenType.RBRACK)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    ExpectToken(TokenType.COMMA);
                    NextToken();
                }
                ret.Values.Add(ParseExpression(1));
            }
            NextToken();
            return ret;
        }

        private ExpressionNode ParseNonBindingExpression()
        {
            switch (Token) {
                case TokenType.LBRACK:
                    return ParseArrayLiteral();
                case TokenType.TRUE_LITERAL:
                case TokenType.FALSE_LITERAL: {
                        BoolLiteral ret = NewNode<BoolLiteral>();
                        ret.Value = TokenType.TRUE_LITERAL == Token ? true : false;
                        NextToken();
                        return ret;
                    }
                case TokenType.STRING:
                    return ParseStringLiteral();
                case TokenType.NUMBER:
                    return ParseNumberLiteral();
                case TokenType.LBRACE:
                    return ParseObjectLiteral();
                case TokenType.LPAREN:
                    return ParseParanthetical();
                case TokenType.ADD:
                case TokenType.SUB:
                case TokenType.NOT:
                    return ParseUnaryExpression();
                case TokenType.PERIOD:
                    return NewNode<ThisExpression>();
                case TokenType.THIS: {
                        ExpressionNode ret = NewNode<ThisExpression>();
                        NextToken();
                        return ret;
                    }
                case TokenType.NULL_LITERAL: {
                        ExpressionNode ret = NewNode<NullLiteral>();
                        NextToken();
                        return ret;
                    }
                case TokenType.NEW:
                    return ParseNewExpression();
                case TokenType.IDENTIFIER:
                    return ParseVariableExpression();
                default:
                    UnexpectedToken();
                    return null;
            }
        }

        private ObjectLookupExpression ParseStaticLookupExpression(ExpressionNode left)
        {
            ObjectLookupExpression ret = NewNode<ObjectLookupExpression>();
            NextToken();
            ExpectToken(TokenType.IDENTIFIER);
            ret.Object = left;
            ret.Lookup = StrVal;
            NextToken();
            return ret;
        }

        private ConditionalExpression ParseConditionalExpression(ExpressionNode left)
        {
            ConditionalExpression ret = NewNode<ConditionalExpression>();
            ret.Condition = left;
            NextToken();
            ret.TrueResult = ParseExpression(2);
            ExpectToken(TokenType.COLON);
            NextToken();
            ret.FalseResult = ParseExpression(2);
            return ret;
        }

        private ArrayLookupExpression ParseDynamicLookupExpression(ExpressionNode left)
        {
            ArrayLookupExpression ret = NewNode<ArrayLookupExpression>();
            ret.Object = left;
            NextToken();
            ret.Lookup = ParseExpression();
            ExpectToken(TokenType.RBRACK);
            NextToken();
            return ret;
        }

        private InvocationExpressionNode ParseInvocationExpression(ExpressionNode left)
        {
            InvocationExpressionNode ret = NewNode<InvocationExpressionNode>();
            NextToken();
            List<ExpressionNode> Arguments = new List<ExpressionNode>();
            bool first = true;
            while (Token != TokenType.RPAREN) {
                if (first) {
                    first = false;
                }
                else {
                    ExpectToken(TokenType.COMMA);
                    NextToken();
                }
                Arguments.Add(ParseExpression(1));
            }
            NextToken();
            ret.Callee = left;
            ret.Arguments = Arguments.ToArray();
            return ret;
        }

        private AssignmentExpression ParseAssignmentExpression(ExpressionNode left)
        {
            if (!left.IsLeftHand)
                Error("Invalid lefthand assignment");
            AssignmentExpression ret = NewNode<AssignmentExpression>();
            ret.Left = left;
            int precedence = TokenPrecedence;
            NextToken();
            ret.Right = ParseExpression(precedence);
            return ret;
        }

        private void GetArrowFunctionArgs(ExpressionNode e, List<string> r)
        {
            if (e is VariableExpression) {
                r.Add(((VariableExpression)e).Value);
            }
            else {
                if (e.Token != TokenType.COMMA) {
                    Error("Malformed argument list for lambda function");
                }
                GetArrowFunctionArgs(((InfixExpression)e).Left, r);
                GetArrowFunctionArgs(((InfixExpression)e).Right, r);
            }

        }

        private ArrowFunction ParseObjectLiteralFunction()
        {
            NextToken();
            ArrowFunction ret = NewNode<ArrowFunction>();
            ret.Token = TokenType.ARROW;
            ret.Body = new FunctionBlock();
            bool first = true;
            while (Token != TokenType.RPAREN) {
                if (first) {
                    first = false;
                }
                else {
                    ExpectToken(TokenType.COMMA);
                    NextToken();
                }
                ExpectToken(TokenType.IDENTIFIER);
                ret.Arguments.Add(StrVal);
                NextToken();
            }
            NextToken();
            ExpectToken(TokenType.LBRACE);
            NextToken();
            while (Token != TokenType.RBRACE)
                ret.Body.Statements.Add(ParseStatement());
            NextToken();
            return ret;
        }

        private ObjectLiteral ParseObjectLiteral()
        {
            ObjectLiteral ret = NewNode<ObjectLiteral>();
            NextToken();
            bool first = true;
            while (Token != TokenType.RBRACE) {
                if (first) {
                    first = false;
                }
                else {
                    ExpectToken(TokenType.COMMA);
                    NextToken();
                }
                switch (Token) {
                    case TokenType.STRING:
                    case TokenType.IDENTIFIER:
                        string str = StrVal;
                        NextToken();
                        if (Token == TokenType.COLON) {
                            NextToken();
                            ret.Properties.Add(new DataPair<string, ExpressionNode>(str, ParseExpression(1)));
                        }
                        else if (Token == TokenType.LPAREN) {
                            ret.Properties.Add(new DataPair<string, ExpressionNode>(str, ParseObjectLiteralFunction()));
                        }
                        else {
                            UnexpectedToken();
                        }
                        break;
                    default:
                        UnexpectedToken();
                        break;
                }
            }

            //check uniqueness of keys
            List<string> Keys = new List<string>();
            foreach (DataPair<string, ExpressionNode> propertie in ret.Properties) {
                if (Keys.IndexOf(propertie.Key) != -1)
                    Error("Duplicate key in object literal");
                Keys.Add(propertie.Key);
            }
            NextToken();
            return ret;
        }

        private ArrowFunction ParseArrowFunction(ExpressionNode left)
        {
            ArrowFunction ret = NewNode<ArrowFunction>();
            int precedence = TokenPrecedence;
            if (left != null) {
                GetArrowFunctionArgs(left, ret.Arguments);
            }
            NextToken();
            if (Token == TokenType.LBRACE) {
                ret.Body = new FunctionBlock();
                NextToken();
                while (Token != TokenType.RBRACE) {
                    ret.Body.Statements.Add(ParseStatement());
                }
                NextToken();
            }
            else {
                ret.Expression = ParseExpression(1);
            }
            return ret;
        }

        private ExpressionNode ParseInfixExpression(ExpressionNode left)
        {
            if (TokenPrecedence == 0) // all infix tokens have binding powers
                UnexpectedToken();

            switch (Token) {
                case TokenType.ARROW:
                    return ParseArrowFunction(left);
                case TokenType.PERIOD:
                    return ParseStaticLookupExpression(left);
                case TokenType.LPAREN:
                    return ParseInvocationExpression(left);
                case TokenType.LBRACK:
                    return ParseDynamicLookupExpression(left);
                case TokenType.CONDITIONAL:
                    return ParseConditionalExpression(left);
                case TokenType.ASSIGN:
                case TokenType.ASSIGN_ADD:
                case TokenType.ASSIGN_DIV:
                case TokenType.ASSIGN_MOD:
                case TokenType.ASSIGN_MUL:
                case TokenType.ASSIGN_SUB:
                    return ParseAssignmentExpression(left);
                default: {
                        InfixExpression ret = NewNode<InfixExpression>();
                        ret.Left = left;
                        int precedence = TokenPrecedence;
                        NextToken();
                        ret.Right = ParseExpression(precedence);
                        return ret;
                    }
            }
        }

        public ExpressionNode ParseExpression(int bindingPower = 0)
        {
            ExpressionNode left = ParseNonBindingExpression();

            while (bindingPower < TokenPrecedence) {
                left = ParseInfixExpression(left);
            }
            return left;
        }
        #endregion

        #region Statements
        private ExpressionStatement ParseExpressionStatement()
        {
            ExpressionStatement ret = NewNode<ExpressionStatement>();
            ret.Expression = ParseExpression();
            ExpectToken(TokenType.SEMICOLON);
            NextToken();
            return ret;
        }

        private WhileStatement ParseWhileStatement()
        {
            WhileStatement ret = NewNode<WhileStatement>();
            NextToken();
            ExpectToken(TokenType.LPAREN);
            NextToken();
            ret.Condition = ParseExpression(0);
            ExpectToken(TokenType.RPAREN);
            NextToken();
            ret.Loop = ParseBlock();
            return ret;
        }

        private VarStatement ParseVarStatement()
        {

            VarStatement ret = NewNode<VarStatement>();
            do {
                NextToken();
                ExpectToken(TokenType.IDENTIFIER);
                string str = StrVal;
                NextToken();
                if (Token == TokenType.ASSIGN) {
                    NextToken();
                    ExpressionNode val = ParseExpression(1);
                    ret.Vars.Add(new DataPair<string, ExpressionNode> { Value = val, Key = str });
                }
                else {
                    ret.Vars.Add(new DataPair<string, ExpressionNode> { Key = str });
                }
            } while (Token == TokenType.COMMA);
            ExpectToken(TokenType.SEMICOLON);
            NextToken();
            return ret;
        }

        private IfStatement ParseIfStatement()
        {
            IfStatement ret = NewNode<IfStatement>();
            NextToken();
            ExpectToken(TokenType.LPAREN);
            NextToken();
            ret.Condition = ParseExpression();
            ExpectToken(TokenType.RPAREN);
            NextToken();
            ret.True = ParseBlock();
            if (Token == TokenType.ELSE) {
                NextToken();
                ret.False = ParseBlock();
            }
            return ret;
        }

        private Block ParseBlock()
        {
            Block ret = NewNode<Block>();
            if (Token == TokenType.LBRACE) {
                NextToken();
                while (Token != TokenType.RBRACE)
                    ret.Statements.Add(ParseStatement());
                NextToken();
            }
            else {
                ret.Statements.Add(ParseStatement());
            }
            return ret;
        }

        private FunctionStatement ParseFunctionStatement()
        {
            FunctionStatement ret = NewNode<FunctionStatement>();
            NextToken();
            ExpectToken(TokenType.IDENTIFIER);
            ret.Name = StrVal;
            NextToken();
            ExpectToken(TokenType.LPAREN);
            NextToken();

            bool first = true;
            while (Token != TokenType.RPAREN) {
                if (first) {
                    first = false;
                }
                else {
                    ExpectToken(TokenType.COMMA);
                    NextToken();
                }
                ExpectToken(TokenType.IDENTIFIER);
                ret.Arguments.Add(StrVal);
                NextToken();
            }
            NextToken();
            ExpectToken(TokenType.LBRACE);
            NextToken();
            while (Token != TokenType.RBRACE)
                ret.Body.Statements.Add(ParseStatement());
            NextToken();
            return ret;
        }

        private ControllStatement ParseControlStatement()
        {
            ControllStatement ret = NewNode<ControllStatement>();
            NextToken();
            if (Token != TokenType.SEMICOLON) {
                ret.Value = ParseExpression(0);
            }
            ExpectToken(TokenType.SEMICOLON);
            NextToken();
            return ret;
        }

        private Statement ParseStatement()
        {
            while (Token == TokenType.SEMICOLON)
                NextToken();

            switch (Token) {
                case TokenType.RETURN:
                case TokenType.BREAK:
                case TokenType.CONTINUE:
                    return ParseControlStatement();
                case TokenType.FUNCTION:
                    return ParseFunctionStatement();
                case TokenType.IF:
                    return ParseIfStatement();
                case TokenType.VAR:
                    return ParseVarStatement();
                case TokenType.WHILE:
                    return ParseWhileStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        public Script ParseScript()
        {
            Script ret = NewNode<Script>();
            while (Token != TokenType.EOF)
                ret.Statements.Add(ParseStatement());
            return ret;
        }
        #endregion
    }
}
