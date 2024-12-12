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
			R visitIfStmt(If stmt);
			R visitPrintStmt(Print stmt);
			R visitWhileStmt(While stmt);
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

		public class If : Stmt
		{
			public Expr condition { get; }
			public Stmt thenBranch { get; }
			public Stmt elseBranch { get; }

			public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
			{
				this.condition = condition;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitIfStmt(this);
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

		public class While : Stmt
		{
			public Expr condition { get; }
			public Stmt body { get; }

			public While(Expr condition, Stmt body)
			{
				this.condition = condition;
				this.body = body;
			}

			public override R accept<R>(Visitor<R> visitor)
			{
				return visitor.visitWhileStmt(this);
			}
		}

	}
}
