namespace CraftingInterpreters.JLox2
{
    public class RuntimeError : Exception
    {
        public Token token { get; }

        public RuntimeError(Token token, string message)
            : base(message)
        {
            this.token = token;
        }
    }
}