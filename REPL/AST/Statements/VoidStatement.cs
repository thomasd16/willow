using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST {
    class VoidStatement : Statement
    {
        public override StatementType Type { get { return StatementType.VOID; } }
        public override void Output(CodeWriter s, int bp) {}
    }
}
