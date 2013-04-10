using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public class Node<T> : INode<T>
    {
        protected List<INode<T>> _childNodes;
        protected List<IAttribute> _attributes;

        public List<INode<T>> Children { get { return _childNodes; } }
        public List<IAttribute> Attribute { get { return _attributes; } }

        public Node()
        {
            _childNodes = new List<INode<T>>();
            _attributes = new List<IAttribute>();
        }        

        public INode<T> AddChild(INode<T> node)
        {
            _childNodes.Add(node);

            return this;
        }

        public void AddAttribute(IAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        public void AddAttribute(List<IAttribute> attribute)
        {
            _attributes.AddRange(attribute);
        }
    }
}
