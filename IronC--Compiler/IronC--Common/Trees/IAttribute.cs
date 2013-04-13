using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public interface IAttribute
    {
        string Name { get; }

        bool IsEqual(string name, object value);
    }
}
