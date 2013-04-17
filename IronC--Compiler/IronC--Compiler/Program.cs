using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common;
using IronC__Generator;
using IronC__Lexical;
using IronC__Syntax;

namespace IronC__Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1. Lexical");
            Console.WriteLine("2. Syntax");
            Console.WriteLine("3. Semantics");
            Console.WriteLine("4. CodeGen");
            Console.Write("Input: ");
            int choose =  Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 1:
                    var reader = new Reader("input.txt");
                    var grammar = reader.ReadGrammar();

                    var la = new LexicalAnalyzer(grammar);

                    var tokens = la.Convert(@"int main() {
                                                          x = 0;
                                                          _abc = 8;  

                                                          if (x == 10) 
                                                            write x;
                                                          else
                                                            ;

                                                          return 0000042
                                                        }");

                    tokens = la.Convert(@"int main() {
                                                  int x;
                                                  int y;
  
                                                  x = 0
                                                  y = 5;

                                                  while (x < y) {
                                                    write x
                                                    writeln;
                                                    x = x + * 1;
                                                  }
                                                }");
                    break;

                case 2:
                    var reader2 = new Reader("input.txt");
                    var grammar2 = reader2.ReadGrammar();

                    var la2 = new LexicalAnalyzer(grammar2);
                    var tokens2 = la2.Convert(@"char c;
                                                int main() {
                                                  int x;
                                                  int y;
  
                                                  x = 0;
                                                  y = 5;

                                                  while (x < y) {
                                                    write x;
                                                    writeln;
                                                    x = x + 1;
                                                  }
                                                }");
                    var syn = new SyntaxAnalyzer(tokens2);
                    syn.Analyze();
                    break;

                case 3:
                    break;

                case 4:
                    var reader3 = new Reader("input.txt");
                    var grammar3 = reader3.ReadGrammar();

                    var la3 = new LexicalAnalyzer(grammar3);
                    /*var tokens3 = la3.Convert(@"char c;
                                                int main() {
                                                  int x;
                                                  int y;
  
                                                  x = 0;
                                                  y = 5;

                                                  while (x < y) {
                                                    write x;
                                                    writeln;
                                                    x = x + 1;
                                                  }
                                                }");*/
                    var tokens3 = la3.Convert(@"int main()
{
char a;
read a;
a = a + 3;
write a;
return 0;
}");
                    var syn3 = new SyntaxAnalyzer(tokens3);
                    var tree3 = syn3.Analyze();

                    var gen = new CodeGenerator(tree3);
                    gen.Generate("app.exe");
                    break;

                default:
                    Console.WriteLine("Error");
                    break;
            }
            Console.ReadKey();
        }
    }
}
