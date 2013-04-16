using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public interface INode
    {
        IList<INode> Children { get; }
        IList<IAttribute> Attribute { get; }
        
        INode AddChild(INode value);

        void AddAttribute(IAttribute attribute);
        void AddAttribute(IEnumerable<IAttribute> attribute);
    }
}
