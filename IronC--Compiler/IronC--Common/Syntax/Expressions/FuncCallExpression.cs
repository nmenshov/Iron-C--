using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class FuncCallExpression: Expression
    {
        public Id Func { get; set; }
        public Expression[] Params { get; set; }

        public FuncCallExpression(Id func, Expression[] parameters)
        {
            Func = func;
            Params = parameters;
            AddAttribute(new IdAttr(func));
            AddChildren(parameters);
        }

        public FuncCallExpression()
        {}
    }
}
