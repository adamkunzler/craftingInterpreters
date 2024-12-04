namespace CraftingInterpreters.JLox2
{
	public abstract class Stmt
	{
		public abstract R accept<R>(Visitor<R> visitor);

		public interface Visitor<R>
		{
			R visitBlockStmt(Block stmt);
			R visitExpressionStmt(Expression stmt);
			R visitVarStmt(Var stmt);
			R visitPrintStmt(Print stmt);
		}

		public class Block : Stmt
		{
			public List<Stmt> statements { get; }

			public Block(List<Stmt> statements)
			{
				this.statements = statements;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitBlockStmt(this);
			}
		}

		public class Expression : Stmt
		{
			public Expr expression { get; }

			public Expression(Expr expression)
			{
				this.expression = expression;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitExpressionStmt(this);
			}
		}

		public class Var : Stmt
		{
			public Token name { get; }
			public Expr initializer { get; }

			public Var(Token name, Expr initializer)
			{
				this.name = name;
				this.initializer = initializer;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitVarStmt(this);
			}
		}

		public class Print : Stmt
		{
			public Expr expression { get; }

			public Print(Expr expression)
			{
				this.expression = expression;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitPrintStmt(this);
			}
		}

	}
}
