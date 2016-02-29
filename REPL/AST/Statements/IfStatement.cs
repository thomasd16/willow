using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class IfStatement : Statement
    {
        public override StatementType Type { get { return StatementType.IF; } }
        public ExpressionNode Condition;
        public Block True;
        public Block False;
        public override void Visit(INodeVisitor Visitor)
        {
            Condition = Visitor.Visit<ExpressionNode>(Condition);
            True = Visitor.Visit<Block>(True);
            if (False != null)
            {
                False  = Visitor.Visit<Block>(False);
            }
        }
        public override void Output(CodeWriter s, int bp) {
            s.Write("if(");
            Condition.Output(s, 0);
            s.Write(") ");
            True.Output(s,0);
            if (False != null) {
                s.Write("else ");
                False.Output(s,0);
            }
        }
    }
}
