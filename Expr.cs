namespace CraftingInterpreters.JLox2
{
	public abstract class Expr
	{
		public abstract R accept<R>(Visitor<R> visitor);

		public interface Visitor<R>
		{
			R visitBinaryExpr(Binary expr);
			R visitGroupingExpr(Grouping expr);
			R visitLiteralExpr(Literal expr);
			R visitUnaryExpr(Unary expr);
		}

		public class Binary : Expr
		{
			public Expr left { get; }
			public Token operatr { get; }
			public Expr right { get; }

			public Binary(Expr left, Token operatr, Expr right)
			{
				this.left = left;
				this.operatr = operatr;
				this.right = right;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitBinaryExpr(this);
			}
		}

		public class Grouping : Expr
		{
			public Expr expression { get; }

			public Grouping(Expr expression)
			{
				this.expression = expression;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitGroupingExpr(this);
			}
		}

		public class Literal : Expr
		{
			public Object value { get; }

			public Literal(Object value)
			{
				this.value = value;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitLiteralExpr(this);
			}
		}

		public class Unary : Expr
		{
			public Token operatr { get; }
			public Expr right { get; }

			public Unary(Token operatr, Expr right)
			{
				this.operatr = operatr;
				this.right = right;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitUnaryExpr(this);
			}
		}

	}
}
