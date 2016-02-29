using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    abstract class Statement: AstNode
    {
        public abstract StatementType Type { get; }
    }
}
