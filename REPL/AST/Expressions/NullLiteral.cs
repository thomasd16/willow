using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class NullLiteral : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.NULL_LITERAL; }
        }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write("null");
        }
    }
}
