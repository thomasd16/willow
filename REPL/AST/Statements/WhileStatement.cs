using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class WhileStatement : Statement
    {
        public override StatementType Type { get { return StatementType.WHILE; } }
        public ExpressionNode Condition;
        public Block Loop;
        public override void Visit(INodeVisitor i)
        {
            Condition = i.Visit<ExpressionNode>(Condition);
            Loop = i.Visit<Block>(Loop);
        }
        public override void Output(CodeWriter s, int bp) {
            s.Write("while(");
            Condition.Output(s, 0);
            s.Write(")");
            Loop.Output(s, 0);
        }
    }
}
