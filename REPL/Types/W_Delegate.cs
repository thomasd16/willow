using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    delegate W_Type W_Delegate(W_Object thisarg, params W_Type[] args);
}
