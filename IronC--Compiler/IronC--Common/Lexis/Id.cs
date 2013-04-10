using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public class Id:Terminal
    {
        public string Value { get; private set; }

        public Id() : base("Id")
        {            
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public override Symbol GetCopy(string str)
        {
            var id = new Id();
            id.SetValue(str);
            return id;
        }
    }
}
