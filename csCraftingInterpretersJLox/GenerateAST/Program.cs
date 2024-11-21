namespace GenerateAST
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Usage: generateAST <output director>");
                return;
            }

            var outputDir = args[0];

            defineAst(outputDir, "Expr", new List<string>
            {
                "Binary     : Expr left, Token operator, Expr right",
                "Grouping   : Expr expression",
                "Literal    : Object value",
                "Unary      : Token operator, Expr right"     
            });
        }

        private static void defineAst(
            string outputDir, 
            string baseName, 
            List<string> types)
        {
            var path = $"{outputDir}/{baseName}.cs";

            using StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("namespace CraftingInterpreters.JLox2");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic abstract class {baseName}");
            writer.WriteLine("\t{");
            
            // the AST classes
            foreach(var type in types) 
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                defineType(writer, baseName, className, fields);
            }

            writer.WriteLine("\t}"); // end abstract class
            writer.WriteLine("}"); // end namespace

            writer.Close();
        }

        private static void defineType(
            StreamWriter writer,
            string baseName,
            string className,
            string fieldList)
        {
            writer.WriteLine($"\t\tstatic class {className} : {baseName}");
            writer.WriteLine("\t\t{");

            // fields/properties            
            var fields = fieldList.Split(",");
            foreach (var field in fields)
            {
                writer.WriteLine($"\t\t\tpublic {field.Trim()} {{ get; }}");
            }

            // constructor
            writer.WriteLine();
            writer.WriteLine($"\t\t\tpublic {className}({fieldList})");
            writer.WriteLine("\t\t\t{");
            
            // store parameters in fields            
            foreach (var field in fields)
            {
                var name = field.Trim().Split(" ")[1];
                writer.WriteLine($"\t\t\t\tthis.{name} = {name};");
            }
            writer.WriteLine("\t\t\t}"); // end constructor
            
            writer.WriteLine("\t\t}"); // end static class
            writer.WriteLine();
        }
    }
}
