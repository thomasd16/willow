using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ObjectLookupExpression: ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.OBJECT_LOOKUP; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Object = Visitor.Visit<ExpressionNode>(Object);
        }
        public override bool IsLeftHand { get { return true; } }
        public ExpressionNode Object;
        public string Lookup;
        public override void Output(CodeWriter s, int bp)
        {
            Object.Output(s, Precedence - 1);
            s.Write('.');
            s.Write(Lookup);
        }
    }
}
