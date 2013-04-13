using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public class Attribute<T>:IAttribute
    {
        protected string _name;

        public string Name { get { return _name; } }
        public T Value { get; private set; }

        public Attribute(string name, T value)
        {
            _name = name;
            Value = value;
        }

        public bool IsEqual(string name, object value)
        {
            var obj = (T)value;
            return obj.Equals(Value);
        }
    }
}
