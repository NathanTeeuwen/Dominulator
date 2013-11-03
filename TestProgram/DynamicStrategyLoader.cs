using Dominion.Strategy;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Program
{
    class DynamicStrategyLoader
    {
        static string strategyOutputFolder = "DominulatorStrategies";       
        PlayerActionAndSource[] playerActions = null;

        public DynamicStrategyLoader()
        {
            InitGetCompiler();
        }

        public bool Load()
        {
            if (!WriteOutAllEmbeddedStrategies())
            {
                return false;
            }

            if (!LoadAllCustomStrategies())
                return false;                        

            return true;
        }

        public IEnumerable<PlayerAction> AllStrategies()
        {
            return this.playerActions.Select(a => a.playerAction);
        }

        public PlayerAction GetPlayerAction(object playerActionOrString)
        {
            if (playerActionOrString is PlayerAction)
                return (PlayerAction)playerActionOrString;
            else if (playerActionOrString is string)
                return GetPlayerAction((string)playerActionOrString);
            else 
                throw new Exception("Must be PlayerAction or string");
        }

        public PlayerAction GetPlayerAction(string name)
        {
            return this.playerActions.Where(playerAction => playerAction.playerAction.name == name).Select(playerAction => playerAction.playerAction).FirstOrDefault();            
        }

        public string GetPlayerSource(string name)
        {
            string fileName = this.playerActions.Where(playerAction => playerAction.playerAction.name == name).Select(action => action.fileName).FirstOrDefault();
            if (fileName == null)
                return "Strategy Not Found";

            try
            {
                using (var reader = new System.IO.StreamReader(fileName))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch
            {
                return "Error loading file: " + name;
            }
        }

        public PlayerAction GetPlayerActionFromCode(string code)
        {
            CompiledResult result = DynamicallyLoadFromSource(code);            
            if (result.assembly == null)
                return null;

            PlayerAction[] actions = GetAllPlayerActions(result.assembly);
            if (actions.Length != 1)
                return null;
                   
            return actions[0];
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

        public bool LoadAllCustomStrategies()
        {
            var result = new List<PlayerActionAndSource>();
            
            string[] files = System.IO.Directory.GetFiles(strategyOutputFolder);
            foreach (string file in files)
            {
                System.Reflection.Assembly assembly = DynamicallyLoadFromFile(file);
                if (assembly == null)
                    return false;

                PlayerAction[] playerActions = GetAllPlayerActions(assembly);
                if (playerActions.Length > 1)
                {
                    System.Console.WriteLine("Warning, multiple strategies in file {0}", file);
                }
                foreach (PlayerAction playerAction in playerActions)
                {
                    result.Add( new PlayerActionAndSource(file, playerAction));
                }
            }

            this.playerActions = result.ToArray();

            return true;
        }

        CSharpCodeProvider provider;
        CompilerParameters CompilerParams;

        void InitGetCompiler()
        {
            CompilerParams = new CompilerParameters();

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";

            string[] references = { "System.dll", "Dominion.dll", "System.Core.dll", "System.Data.Dll", "Dominion.Strategy.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);

            provider = new CSharpCodeProvider();            
        }

        public class CompiledResult
        {
            public readonly System.Reflection.Assembly assembly;
            public readonly CompilerError error;

            public CompiledResult(System.Reflection.Assembly assembly, CompilerError error)
            {
                this.assembly = assembly;
                this.error = error;
            }
        }

        public CompiledResult DynamicallyLoadFromSource(string code)
        {
            CompilerError error = null;
            System.Reflection.Assembly assembly = null;

            CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);

            if (compile.Errors.HasErrors)
            {
                error = compile.Errors[0];
                System.Console.WriteLine("Compile error:");
                foreach (CompilerError ce in compile.Errors)
                {
                    string strError = ce.ToString();
                    System.Console.WriteLine(strError);                   
                }
            }
            else
            {
                assembly = compile.CompiledAssembly;
            }

            return new CompiledResult(assembly, error);
        }

        public System.Reflection.Assembly DynamicallyLoadFromFile(params string[] sourceFiles)
        {            
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

        class PlayerActionAndSource
        {
            public PlayerActionAndSource(string fileName, PlayerAction playerAction)
            {
                this.fileName = fileName;
                this.playerAction = playerAction;
            }

            public readonly string fileName;
            public readonly PlayerAction playerAction;
        }
    }
}