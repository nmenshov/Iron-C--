using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    [XmlInclude(typeof(Block))]
    [XmlInclude(typeof(BreakStatement))]
    [XmlInclude(typeof(Expression))]
    [XmlInclude(typeof(IfStatement))]
    [XmlInclude(typeof(ReadStatement))]
    [XmlInclude(typeof(ReturnStatement))]
    [XmlInclude(typeof(WhileStatement))]
    [XmlInclude(typeof(WriteStatement))]
    [XmlInclude(typeof(WritelnStatement))]
    public abstract class Statement: Node
    {
    }
}
