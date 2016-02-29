using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    /*
     * Todo refactor this entire object
     */
    class ObjectLiteral: ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.OBJECT_LITERAL; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            for (int i = 0; i < Properties.Count; i++)
                Properties[i].Value = Visitor.Visit<ExpressionNode>(Properties[i].Value);
        }
        public List<DataPair<string, ExpressionNode>> Properties = new List<DataPair<string, ExpressionNode>>();
        public override void Output(CodeWriter s, int bp) {
            s.Write('{');
            s.Indent();
            foreach (DataPair<string, ExpressionNode> Property in Properties) {
                s.NewLine();
                s.Write('"');
                foreach (char x in Property.Key.ToCharArray()) {
                    switch (x) {
                        case '\b': s.Write("\\b"); break;
                        case '\f': s.Write("\\f"); break;
                        case '\\': s.Write("\\\\"); break;
                        case '\n': s.Write("\\n"); break;
                        case '\r': s.Write("\\r"); break;
                        case '\t': s.Write("\\t"); break;
                        default: s.Write(x); break;
                    }
                }
                s.Write('"');
                s.Write(':');
                Property.Value.Output(s, 0);
                if (Property.Key != Properties.Last().Key)
                    s.Write(',');
            }
            s.Outdent();
            s.NewLine();
            s.Write('}');
        }
    }
}
