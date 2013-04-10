using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Generator;

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
            int choose = Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 1:
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
