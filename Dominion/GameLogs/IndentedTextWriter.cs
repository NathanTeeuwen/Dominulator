using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class IndentedTextWriter
        : IDisposable
    {
        private bool isNewLine = true;
        int indentLevel = 0;
        System.IO.TextWriter textWriter;

        public IndentedTextWriter(string filename)
        {
            this.textWriter = new System.IO.StreamWriter(filename);
        }

        public void Indent()
        {
            this.indentLevel++;
        }

        public void Unindent()
        {
            this.indentLevel--;
        }

        public void Write(string format, params object[] args)
        {
            IndentIfNewLine();
            this.textWriter.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            IndentIfNewLine();
            if (args.Length == 0)
            {
                this.textWriter.WriteLine("{0}", format);
            }
            else
            {
                this.textWriter.WriteLine(format, args);
            }
            this.isNewLine = true;
        }

        public void WriteLine()
        {
            this.textWriter.WriteLine();
            this.isNewLine = true;
        }

        private void IndentIfNewLine()
        {
            if (this.isNewLine)
            {
                for (int i = 0; i < this.indentLevel; ++i)
                {
                    this.textWriter.Write("  ");
                }

                this.isNewLine = false;
            }
        }

        public void Dispose()
        {
            this.textWriter.Dispose();
        }
    }


}
