using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class StringLiteral : ExpressionNode
    {
        public string Value;
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.STRING_LITERAL; }
        }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write('"');
            foreach (char x in Value.ToCharArray()) {
                switch (x) {
                    case '"': s.Write("\\\""); break;
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
        }
    }
}
