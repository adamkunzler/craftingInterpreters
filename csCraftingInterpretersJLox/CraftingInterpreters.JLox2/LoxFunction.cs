
namespace CraftingInterpreters.JLox2
{
    public class LoxFunction : LoxCallable
    {
        private Stmt.Function declaration;
        private Environment closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure; 
        }

        public int arity()
        {
            return declaration.parameters.Count;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(closure);

            for(var i = 0; i < declaration.parameters.Count; i++)
            {
                environment.define(declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.executeBlock(declaration.body, environment);
            }
            catch(Return returnValue)
            {
                return returnValue.value;
            }
            
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.name.lexeme}>";
        }
    }
}