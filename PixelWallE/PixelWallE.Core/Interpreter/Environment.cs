using System.Collections.Generic;

namespace PixeLWallE

{
    public class Environment
    {
        public Dictionary<string, object> VarMap = new();

        public void Define(string varId, object value)
        {
            VarMap[varId] = value;
        }
        public object Get(Token var)
        {
            if (VarMap.TryGetValue(var.Value, out var value))
            {
                return value;
            }
            throw new RuntimeErrorException(var, $"Variable not found: {var.Value}");
        }
    }
}