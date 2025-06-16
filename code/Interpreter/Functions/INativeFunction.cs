public interface INativeFunction
{
    object Invoke(List<object> args);
    public int Arity { get; }
}