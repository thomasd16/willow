using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL
{
    class SyntaxError: Exception
    {
        string val;
        int Line;
        public SyntaxError(string message, int line)
        {
            val = message;
            Line = line;
        }
        public override string Message
        {
            get
            {
                return this.ToString();
            }
        }
        public override string ToString()
        {
            return Output("String");
        }
        public string Output(string fileName)
        {
            return String.Format("{0}[{1}] : {2}", fileName, Line, val);
        }
    }
}
