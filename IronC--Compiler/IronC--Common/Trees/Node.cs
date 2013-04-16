using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public class Node : INode
    {
        protected List<INode> _childNodes;
        protected List<IAttribute> _attributes;

        public IList<INode> Children { get { return _childNodes; } }
        public IList<IAttribute> Attribute { get { return _attributes; } }

        public Node()
        {
            _childNodes = new List<INode>();
            _attributes = new List<IAttribute>();
        }        

        public INode AddChild(INode node)
        {
            _childNodes.Add(node);

            return this;
        }

        public void AddChildren(IEnumerable<INode> nodes)
        {
            _childNodes.AddRange(nodes);
        }

        public void AddAttribute(IAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        public void AddAttribute(IEnumerable<IAttribute> attribute)
        {
            _attributes.AddRange(attribute);
        }
    }
}
