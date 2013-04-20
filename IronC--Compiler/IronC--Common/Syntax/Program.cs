using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class Program: Node
    {
        public VarDeclaration[] VarDeclarations { get; set; }
        public FuncDeclaration[] FuncDeclarations { get; set; }

        public Program(VarDeclaration[] varDeclarations, FuncDeclaration[] funcDeclarations)
        {
            VarDeclarations = varDeclarations;
            FuncDeclarations = funcDeclarations;
            AddChildren(VarDeclarations);
            AddChildren(FuncDeclarations);
        }

        public Program()
        {
        }

        public void Serialize(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(Program));
                serializer.Serialize(fs, this);
            }
        }
    }
}
