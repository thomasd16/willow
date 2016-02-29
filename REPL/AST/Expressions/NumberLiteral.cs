using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class NumberLiteralNode : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.NUMBER_LITERAL; }
        }
        public double Value;
        public override void Output(CodeWriter s, int bp)
        {
            s.Write(Value.ToString());
        }
    }
}
