using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public interface IValidator
    {
        List<string> Validate(ITree tree);
    }
}
