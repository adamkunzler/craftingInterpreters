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
                "Assign     : Token name, Expr value",
                "Binary     : Expr left, Token operatr, Expr right",
                "Call       : Expr callee, Token paren, List<Expr> arguments",
                "Grouping   : Expr expression",
                "Literal    : object value",
                "Variable   : Token name",
                "Logical    : Expr left, Token operatr, Expr right",
                "Unary      : Token operatr, Expr right"     
            });

            defineAst(outputDir, "Stmt", new List<string>
            {
                "Block      : List<Stmt> statements",
                "Expression : Expr expression",
                "Var        : Token name, Expr initializer",
                "If         : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "Print      : Expr expression",
                "While      : Expr condition, Stmt body"
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
            writer.WriteLine("\t\tpublic abstract R accept<R>(Visitor<R> visitor);");
            writer.WriteLine();
            
            defineVisitor(writer, baseName, types);

            // the AST classes
            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                defineType(writer, baseName, className, fields);
            }

            writer.WriteLine("\t}"); // end abstract class
            writer.WriteLine("}"); // end namespace

            writer.Close();
        }

        private static void defineVisitor(
            StreamWriter writer,
            string baseName, 
            List<string> types)
        {
            writer.WriteLine("\t\tpublic interface Visitor<R>");
            writer.WriteLine("\t\t{");
            foreach(var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                writer.WriteLine($"\t\t\tR visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }
            writer.WriteLine("\t\t}"); // end interface
            writer.WriteLine();
        }

        private static void defineType(
            StreamWriter writer,
            string baseName,
            string className,
            string fieldList)
        {
            writer.WriteLine($"\t\tpublic class {className} : {baseName}");
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

            // visitor pattern
            writer.WriteLine();
            writer.WriteLine("\t\t\tpublic override R accept<R>(Visitor<R> visitor)");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine($"\t\t\t\treturn visitor.visit{className}{baseName}(this);");
            writer.WriteLine("\t\t\t}"); // end accept<R>

            writer.WriteLine("\t\t}"); // end static class
            writer.WriteLine();
        }
    }
}
