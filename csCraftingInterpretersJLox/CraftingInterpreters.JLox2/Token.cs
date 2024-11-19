namespace CraftingInterpreters.JLox2
{
    public class Token
    {
        public TokenType type { get; }
        public string lexeme { get; }
        public object literal { get; }
        public int line { get; }

        public Token(
            TokenType type,
            string lexeme,
            object literal,
            int line
        )
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }
}