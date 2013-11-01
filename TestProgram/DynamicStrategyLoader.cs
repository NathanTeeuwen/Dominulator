using System;
using System.Collections.Generic;
using System.Text;

using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using System.Reflection;

namespace Program
{
    class StrategyLoader
    {
        public static PlayerAction DynamicallyLoad(string code)
        {
            CompilerParameters CompilerParams = new CompilerParameters();            

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";            

            string[] references = { "System.dll", "TestProgram.cs.exe", "Dominion.dll", "System.Core.dll", "System.Data.Dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);

            if (compile.Errors.HasErrors)
            {
                var builder = new System.Text.StringBuilder();
                builder.AppendLine("Compile error:");                
                foreach (CompilerError ce in compile.Errors)
                {
                    builder.AppendLine(ce.ToString());
                }
                System.Console.WriteLine(builder.ToString());
                return null;
            }

            //ExpoloreAssembly(compile.CompiledAssembly);

            Module module = compile.CompiledAssembly.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
            {
                mt = module.GetType("Program.Strategies.TreasureMapDoctor");
            }

            if (mt != null)
            {
                methInfo = mt.GetMethod("Player");
            }

            if (methInfo != null)
            {
                PlayerAction playerAction = (PlayerAction)methInfo.Invoke(null, new object[] {});
                return playerAction;
            }

            return null;
        }       
    }
}