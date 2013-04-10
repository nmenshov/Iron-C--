using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public interface ITree<T>
    {
        INode<T> Root { get; }
    }
}
