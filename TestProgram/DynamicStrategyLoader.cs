using Dominion.Strategy;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Program
{
    class StrategyLoader
    {
        static string strategyOutputFolder = "DominulatorStrategies";

        PlayerAction[] playerActions = null;

        public StrategyLoader()
        {           
        }

        public bool Load()
        {
            if (!WriteOutAllEmbeddedStrategies())
            {
                return false;
            }
            
            var loadedAssembly = LoadAllCustomStrategies();
            if (loadedAssembly == null)
                return false;

            this.playerActions = GetAllPlayerActions(loadedAssembly);

            return true;
        }

        public PlayerAction GetStrategy(string name)
        {
            return this.playerActions.Where(playerAction => playerAction.name == name).FirstOrDefault();
        }

        public static PlayerAction[] GetAllPlayerActions(System.Reflection.Assembly assembly)
        {
            var result = new List<PlayerAction>();
            
            foreach (Type innerType in assembly.GetTypes())
            {
                if (!innerType.IsClass)
                    continue;

                if (innerType.Namespace != "Strategies")
                    continue;

                System.Reflection.MethodInfo playerMethodInfo = innerType.GetMethod("Player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (playerMethodInfo == null)
                    continue;

                if (playerMethodInfo.ContainsGenericParameters)
                    continue;

                if (playerMethodInfo.GetParameters().Length > 0)
                {
                    continue;
                }

                PlayerAction playerAction = playerMethodInfo.Invoke(null, new object[0]) as PlayerAction;
                if (playerAction == null)
                    continue;

                result.Add(playerAction);
            }

            return result.ToArray();
        }

        public static bool CreateStrategiesFolderIfNecessary()
        {
            if (!System.IO.Directory.Exists(strategyOutputFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(strategyOutputFolder);
                }
                catch
                {
                }
            }

            return System.IO.Directory.Exists(strategyOutputFolder);                
        }

        public static bool WriteOutAllEmbeddedStrategies()
        {
            string strategyResourcePath = "Program.Strategies.";

            if (!CreateStrategiesFolderIfNecessary())
                return false;

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            foreach (string sourcefile in assembly.GetManifestResourceNames())
            {
                if (!sourcefile.StartsWith(strategyResourcePath))
                {
                    continue;
                }

                string fileContents = Resources.GetEmbeddedContent("", sourcefile);
                string fileName = sourcefile.Remove(0, strategyResourcePath.Length);

                try
                {
                    using (var textWriter = new System.IO.StreamWriter(strategyOutputFolder + "\\" + fileName))
                    {
                        textWriter.Write(fileContents);
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        public static System.Reflection.Assembly LoadAllCustomStrategies()
        {
            string[] files = System.IO.Directory.GetFiles(strategyOutputFolder);
            return DynamicallyLoadFromFile(files);
        }

        public static System.Reflection.Assembly DynamicallyLoadFromFile(params string[] sourceFiles)
        {
            CompilerParameters CompilerParams = new CompilerParameters();            

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";            

            string[] references = { "System.dll", "Dominion.dll", "System.Core.dll", "System.Data.Dll", "Dominion.Strategy.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromFile(CompilerParams, sourceFiles);

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

            return compile.CompiledAssembly;
        }       
    }
}