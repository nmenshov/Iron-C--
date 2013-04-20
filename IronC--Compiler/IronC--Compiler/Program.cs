using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common;
using IronC__Generator;
using IronC__Lexical;
using IronC__Semantics;
using IronC__Syntax;

namespace IronC__Compiler
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("There is no file to compile.");
                return;
            }

            var fileName = args.First();
            if (!File.Exists(fileName))
            {
                Console.WriteLine("There is no file to compile.");
                return;
            }

            string program;
            using (var fr = new StreamReader(File.Open(fileName, FileMode.Open)))
            {
                program = fr.ReadToEnd();
            }

            var reader = new Reader("input.txt");
            var grammar = reader.ReadGrammar();

            var lexical = new LexicalAnalyzer(grammar, "LA.xml");
            var tokens = lexical.Convert(program);

            var syntax = new SyntaxAnalyzer(tokens);
            var tree = syntax.Analyze();

            if (ReportError(syntax.Errors))
                return;

            var semantics = new SemanticAnalyzer(tree);
            semantics.DecorateAndValidateTree();

            if (ReportError(semantics.Errors))
                return;

            var gen = new CodeGenerator(tree);
            gen.Generate(Path.GetFileNameWithoutExtension(fileName) + ".exe");
        }

        private static bool ReportError(List<string> errors)
        {
            if (errors.Any())
            {
                errors.ForEach(Console.WriteLine);
                return true;
            }
            return false;
        }
    }
}
