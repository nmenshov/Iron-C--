using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Syntax
{
    public class ReturnStatement: Statement
    {
        public Expression Expression { get; private set; }

        public ReturnStatement(Expression expression)
        {
            Expression = expression;

            AddChild(expression);
        }
    }
}
