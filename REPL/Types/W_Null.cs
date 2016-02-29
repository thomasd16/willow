using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    [TypeName("Null")]
    class W_Null : W_Type
    {
        public override string ToString()
        {
            return "Null";
        }
        public override bool Bool
        {
            get
            {
                return false;
            }
        }
    }
}
