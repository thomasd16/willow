using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ControllStatement : Statement
    {
        public override StatementType Type { get { return StatementType.CONTROL; } }
        public ExpressionNode Value;
        public override void Visit(INodeVisitor Visitor)
        {
            if (Value != null)
               Value= Visitor.Visit<ExpressionNode>(Value);
        }
        public override void Output(CodeWriter s, int bp) {
            s.Write(TokenHelper.Value(Token));
            if (Value != null) {
                s.Write(' ');
                Value.Output(s, 0);
            }
            s.Write(";");
        }
    }
}
