using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class WhileStatement: Statement
    {
        public Expression Condition { get; set; }
        public Statement Statement { get; set; }

        public WhileStatement(Expression condition, Statement statement)
        {
            Condition = condition;
            Statement = statement;

            AddChild(condition);
            AddChild(statement);
        }

        public WhileStatement()
        {}
    }
}
