using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class FunctionStatement: Statement
    {
        public override StatementType Type { get { return StatementType.FUNCTION; } }
        public List<string> Arguments = new List<string>();
        public override void Visit(INodeVisitor Visitor)
        {
            //Body = Visitor.Visit<FunctionBlock>(Body);
        }
        public FunctionBlock Body = new FunctionBlock();
        public string Name;
        public override void Output(CodeWriter s, int bp) {
            s.Write("function ");
            s.Write(Name);
            s.Write('(');
            s.Write(Arguments.DefaultIfEmpty("").Aggregate((l, r) => l + "," + r));
            s.Write(") ");
            Body.Output(s, 0);
        }
        public ArrowFunction ToArrowFunction() {
            ArrowFunction ret = To<ArrowFunction>();
            ret.Arguments = Arguments;
            ret.Body = Body;
            return ret;
        }
    }
}
