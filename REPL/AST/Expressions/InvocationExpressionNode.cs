using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class InvocationExpressionNode : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.INVOCATION; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Callee = Visitor.Visit<ExpressionNode>(Callee);
            for (int i = 0; i < Arguments.Length; i++)
                Arguments[i] = Visitor.Visit<ExpressionNode>(Arguments[i]);
        }
        public ExpressionNode Callee;
        public ExpressionNode[] Arguments;
        public override void Output(CodeWriter s, int bp)
        {
            Callee.Output(s, Precedence - 1);
            s.Write('(');
            for (int i = 0; i < Arguments.Length; i++)
            {
                Arguments[i].Output(s, 1);
                if (i+1 != Arguments.Length)
                {
                    s.Write(',');
                }

            }
            s.Write(')');
        }
    }
}
