using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class Block: Statement
    {
        public VarDeclaration[] VarDeclarations { get; set; }

        public Statement[] Statements { get; set; }

        public Block(VarDeclaration[] varDeclarations, Statement[] statements)
        {
            VarDeclarations = varDeclarations;
            Statements = statements;
            AddChildren(varDeclarations);
            AddChildren(statements);
        }

        public Block()
        {}
    }
}
