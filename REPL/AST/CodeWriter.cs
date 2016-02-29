using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace REPL.AST
{
    class CodeWriter
    {
        TextWriter Writer;
        int Indentation = 0;
        public CodeWriter(TextWriter writer) {
            Writer = writer;
        }
        public CodeWriter() {
            Writer = new StringWriter();
        }

        public void NewLine() {
            Writer.Write("\n");
            for (int i = 0; i < Indentation; i++ )
                Writer.Write("    ");
        }

        public void Indent() {
            Indentation++;
        }
        public void Outdent() {
            Indentation--;
        }
        public void Write(char data) {
            Writer.Write(data);
        }
        public void Write(String data) {
            Writer.Write("{0}", data);
        }
        public void Write(String format, params object[] data) {
            Writer.Write(format, data);
        }
        public override string ToString() {
            return Writer.ToString();
        }
    }
}
