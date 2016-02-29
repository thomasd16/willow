using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Value;
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.UNARY; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Value = Visitor.Visit<ExpressionNode>(Value);
        }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write(TokenHelper.Value(Token));
            Value.Output(s, 13);
        }
    }
}
