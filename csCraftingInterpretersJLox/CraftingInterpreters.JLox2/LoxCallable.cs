namespace CraftingInterpreters.JLox2
{
    public interface LoxCallable
    {
        int arity();
        object call(Interpreter interpreter, List<object> arguments);
    }
}