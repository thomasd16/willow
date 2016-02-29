using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    [TypeName("Number")]
    class W_Number : W_Type
    {
        private double Value;
        public W_Number(double val)
        {
            Value = val;
        }
        public override double Number
        {
            get
            {
                return Value; 
            }
        }
        public override bool Bool
        {
            get
            {
                return Value != 0;
            }
        }
        public override string String
        {
            get
            {
                return Value.ToString();
            }
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
