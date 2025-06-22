using System.Collections.Generic;

namespace PixeLWallE
{
    public interface INativeFunction
    {
        object Invoke(List<object> args);
        public int Arity { get; }
    }
}