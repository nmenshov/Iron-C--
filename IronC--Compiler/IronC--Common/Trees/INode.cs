using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public interface INode<T>
    {
        List<INode<T>> Children { get; }
        List<IAttribute> Attribute { get; }
        
        INode<T> AddChild(INode<T> value);

        void AddAttribute(IAttribute attribute);
        void AddAttribute(List<IAttribute> attribute);
    }
}
