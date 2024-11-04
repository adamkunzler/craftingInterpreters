using System.Runtime.InteropServices;
using System.Text;

namespace CraftingInterpreters.JLox2
{
    internal class Lox
    {
        public static bool hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                return;
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
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
