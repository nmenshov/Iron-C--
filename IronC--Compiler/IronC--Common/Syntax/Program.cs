using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class Program: Node
    {
        public VarDeclaration[] VarDeclarations { get; private set; }
        public FuncDeclaration[] FuncDeclarations { get; private set; }

        public Program(VarDeclaration[] varDeclarations, FuncDeclaration[] funcDeclarations)
        {
            VarDeclarations = varDeclarations;
            FuncDeclarations = funcDeclarations;
            AddChildren(VarDeclarations);
            AddChildren(FuncDeclarations);
        }
    }
}
