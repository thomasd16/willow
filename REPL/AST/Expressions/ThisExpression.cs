using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ThisExpression: ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.THIS; }
        }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write("this");
        } 
    }
}
