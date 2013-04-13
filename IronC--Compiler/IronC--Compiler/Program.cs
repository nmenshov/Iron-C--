using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common;
using IronC__Generator;
using IronC__Lexical;

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
            int choose = 1;// Convert.ToInt32(Console.ReadLine());

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
                    break;

                case 3:
                    break;

                case 4:
                    var gen = new CodeGenerator();
                    gen.Generate();
                    break;

                default:
                    Console.WriteLine("Error");
                    break;
            }
            Console.ReadKey();
        }
    }
}
