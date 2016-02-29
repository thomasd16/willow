using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ArrayLiteral : ExpressionNode
    {
        public List<ExpressionNode> Values = new List<ExpressionNode>();
        public override ExpressionNodeType Type { get { return ExpressionNodeType.ARRAY_LITERAL; } }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write('[');
            for (int i = 0; i < Values.Count; i++)
            {
                if (i != 0)
                    s.Write(',');
                Values[i].Output(s, 1);
            }
            s.Write(']');
        }
        public override void Visit(INodeVisitor visitor)
        {
            for (int i = 0; i < Values.Count; i++)
                Values[i] = visitor.Visit<ExpressionNode>(Values[i]);
        }
    }
}
