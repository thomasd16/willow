using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    abstract class AstNode
    {
        public int Line;
        public TokenType Token;
        public abstract void Output(CodeWriter s, int bp);
        public T To<T>() where T : AstNode,new() {
            return new T {
                Line = Line,
                Token = Token
            };
        }
        public void Error(string message) {
            throw new SyntaxError(message, Line);
        }
        public override string ToString() {
            CodeWriter sb = new CodeWriter();
            Output(sb, 0);
            return sb.ToString();
        }
        public virtual void Visit(INodeVisitor i){}

    }
}
