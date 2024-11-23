using System.Text;

namespace CraftingInterpreters.JLox2
{
    public class Lox
    {
        public static bool hadError = false;

        static void Main(string[] args)
        {
            var expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(123)
                ),
                new Token(TokenType.STAR, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal(45.67))
                );

            Console.WriteLine(new AstPrinter().print(expression));
            //if (args.Length > 1)
            //{
            //    Console.WriteLine("Usage: jlox [script]");
            //    return;
            //}
            //else if (args.Length == 1)
            //{
            //    runFile(args[0]);
            //}
            //else
            //{
            //    runPrompt();
            //}
        }

        private static void runFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            run(Encoding.Default.GetString(bytes));
            if (hadError) return;
        }

        private static void runPrompt()
        {
            while (true)
            {
                Console.WriteLine("> ");

                var line = Console.ReadLine();
                if (line == null) break;

                run(line);
                hadError = false;
            }
        }

        private static void run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();

            // for now, just print the tokens
            foreach (var t in tokens)
            {
                Console.WriteLine(t);
            }
        }

        public static void error(int line, string message)
        {
            report(line, "", message);
        }

        public static void report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}
