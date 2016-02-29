using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types {
    [TypeName("Array")]
    class W_Array : W_Type {
        List<W_Type> Values = new List<W_Type>();
        public W_Array(IEnumerable<W_Type> source) { Values = source.ToList(); }
        public W_Type this[int index]
        {
            get { return Values[index]; }
            set { Values[index] = value; }
        }
    }
}
