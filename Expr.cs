namespace CraftingInterpreters.JLox2
{
	public abstract class Expr
	{
		static class Binary : Expr
		{
			public Expr left { get; }
			public Token operator { get; }
			public Expr right { get; }

			public Binary(Expr left, Token operator, Expr right)
			{
				this.left = left;
				this.operator = operator;
				this.right = right;
			}
		}

		static class Grouping : Expr
		{
			public Expr expression { get; }

			public Grouping(Expr expression)
			{
				this.expression = expression;
			}
		}

		static class Literal : Expr
		{
			public Object value { get; }

			public Literal(Object value)
			{
				this.value = value;
			}
		}

		static class Unary : Expr
		{
			public Token operator { get; }
			public Expr right { get; }

			public Unary(Token operator, Expr right)
			{
				this.operator = operator;
				this.right = right;
			}
		}

	}
}
