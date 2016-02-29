using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class AssignmentExpression : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.ASSIGNMENT; }
        }
        public ExpressionNode Left;
        public ExpressionNode Right;
        public override void Output(CodeWriter s, int bp) {
            if (Precedence <= bp) {
                s.Write('(');
            }
            Left.Output(s, Precedence - 1);
            s.Write(TokenHelper.Value(Token));
            Right.Output(s, Precedence);
            if (Precedence <= bp) {
                s.Write(')');
            }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Left = Visitor.Visit<ExpressionNode>(Left);
            Right = Visitor.Visit<ExpressionNode>(Right);
        }
    }
}
