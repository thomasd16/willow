using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    [TypeName("Function")]
    class W_Function: W_Type
    {
        public W_Function(W_Delegate val)
        {
            Value = val;
        }
        private W_Delegate Value;
        public override W_Delegate Function
        {
            get
            {
                return Value;
            }
        }
        public override string ToString()
        {
            return "Function";
        }
    }
}
