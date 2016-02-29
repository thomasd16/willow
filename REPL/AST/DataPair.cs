using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class DataPair<Tkey,TValue>
    {
        public DataPair() { }
        public DataPair(Tkey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        public Tkey Key;
        public TValue Value;
    }
}
