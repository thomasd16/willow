using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ExpressionStatement: Statement
    {
        public override StatementType Type { get { return StatementType.EXPRESSION; } }
        public ExpressionNode Expression;
        public override void Visit(INodeVisitor Visitor)
        {
            Expression = Visitor.Visit<ExpressionNode>(Expression);
        }
        public override void Output(CodeWriter s, int bp) {
            Expression.Output(s, 0);
            s.Write(';');
        }
    }
}
