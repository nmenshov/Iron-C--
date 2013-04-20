using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Syntax;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public class TypeValidator:RecursionValidator<object>
    {
        protected override object GetStartValue()
        {
            return new object();
        }

        protected override void OperationBefore(INode parent, INode current, ref object data)
        {

        
        }

        protected override void OperationAfter(INode parent, INode current, ref object data)
        {
           
        }
    }
}
