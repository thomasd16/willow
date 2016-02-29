using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    [TypeName("Bool")]
    class W_Bool: W_Type
    {
        bool Value;
        public W_Bool(bool val)
        {
            Value = val;
        }
        public override string String
        {
            get
            {
                return Value ? "true" : "false";
            }
        }
        public override bool Bool
        {
            get
            {
                return Value;
            }
        }
    }
}
