using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class UnaryExpression: Expression
    {
        public Terminal Operator { get; private set; }
        public Expression Expression { get; private set; }

        public UnaryExpression(Terminal op, Expression expression)
        {
            Operator = op;
            Expression = expression;
            AddAttribute(new OperatorAttr(op));
            AddChild(expression);
        }
    }
}
