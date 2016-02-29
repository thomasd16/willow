using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ArrayLookupExpression : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.ARRAY_LOOKUP; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Object = Visitor.Visit<ExpressionNode>(Object);
            Lookup = Visitor.Visit<ExpressionNode>(Lookup);
        }
        public override bool IsLeftHand { get { return true; } }
        public ExpressionNode Object;
        public ExpressionNode Lookup;
        public override void Output(CodeWriter s, int bp)
        {
            Object.Output(s, Precedence-1);
            s.Write('[');
            Lookup.Output(s, 0);
            s.Write(']');
        }
    }
}
