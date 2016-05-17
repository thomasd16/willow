using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
//using REPL.AST;
using System.IO;
using REPL.AST;
using REPL.Types;
namespace REPL
{
    class Program
    {
        public static double fib(double i)
        {
            if (i < 2)
                return 1;
            else
                return fib(i - 1) + fib(i - 2);
        }
        public static void Main(string[] args)
        {
            TextReader source = File.OpenText(@"..\..\..\script.willow");
            
            //parses the script and sets up the AST
            Parser parser = new Parser(source);
            
            //astNode representing a script
            Script script = parser.ParseScript();
            //Console.WriteLine(script);
            //Function is an AST node that holds aditional information for 
            //compiling function does scope resolution and a few optomizations
            Function function = new Function(script);
            function.Process();
            
            //Note the function which is the final post processed AST
            //has a toString Method that returns a script that is functionally identical to the input
            //Console.WriteLine(function);
            
            
            //W_Object is simply a dictionary that holds W_Type's
            //W_Object implements prototype based inheritance
            W_Object contextObject = new W_Object();

            //Exporting a function to an object
            //most of this function is boilerplate eventualy 
            //I will write an object to export native C# functions and objects
            //into W_Objects using reflection
            contextObject["print"] = new W_Function((thisarg, arguments) =>
            {
             
                Console.WriteLine(arguments[0].String);
                return W_Type.NULL;
            });
            contextObject["printf"] = new W_Function((thisarg, arguments) =>
            {
                string format = arguments[0].String;
                Console.WriteLine(format, arguments.Skip(1).Select(x => x.String).ToArray());
                return W_Type.NULL;
            });
            contextObject["readNumber"] = new W_Function((thisarg, arguments) =>
            {
                int retval;
                return int.TryParse(Console.ReadLine(), out retval) ? new W_Number((double)retval) : W_Type.NULL;
                
            });

            //Finaly the script is compiled into a delgate
            W_Script del = Compiler.CompileScript(function);

            //And then the script can be executed
            del(contextObject);
        }
    }
}
