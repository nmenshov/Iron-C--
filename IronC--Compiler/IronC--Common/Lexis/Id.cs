using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public class Id:Terminal
    {
        public string Value { get;  set; }

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
        public override bool IsEqual(string str)
        {
            if (str.Length == 0)
                return false;

            if (!(str[0] == '_' && str.Length>1 || char.IsLetterOrDigit(str[0])))
                return false;

            if (str.Any(x => !char.IsLetterOrDigit(x) && x != '_'))
                return false;

            return true;
        }
    }
}
