using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Syntax.Expressions;

namespace IronC__Common.Syntax
{
    [XmlInclude(typeof(BinaryExpression))]
    [XmlInclude(typeof(FuncCallExpression))]
    [XmlInclude(typeof(GetArrayValueExpression))]
    [XmlInclude(typeof(GetValueExpression))]
    [XmlInclude(typeof(NumberExpression))]
    [XmlInclude(typeof(SetArrayValueExpression))]
    [XmlInclude(typeof(SetValueExpression))]
    [XmlInclude(typeof(UnaryExpression))]
    public abstract class Expression: Statement
    {
    }
}
