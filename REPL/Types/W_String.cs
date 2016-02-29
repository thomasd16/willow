using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REPL;
namespace REPL.Types
{
    [TypeName("String")]
    class W_String : W_Type
    {
        string Value;
        public W_String(string val)
        {
            Value = val;
        }
        public override string String
        {
            get
            {
                return Value;
            }
        }
        public override W_Object Object
        {
            get
            {
                W_Object retval = new W_Object(CoreApi.StringPrototype);
                retval["value"] = this;
                return retval;
            }
        }
        public override string ToString()
        {
            return Value;
        }

        public override bool Bool
        {
            get
            {
                return Value.Length != 0;
            }
        }
    }
}
