using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion.Strategy;

namespace BuiltInStrategies
{
    public static class StrategyLoader
    {

        public static PlayerAction PlayerFromString(string playerStrategy)
        {
            return GetAllPlayerActions().Where(player => player.name == playerStrategy).FirstOrDefault();
        }

        public static PlayerAction[] GetAllPlayerActions()
        {
            return GetAllPlayerActions(System.Reflection.Assembly.GetExecutingAssembly());
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
    }
}
