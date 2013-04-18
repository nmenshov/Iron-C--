using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public class Num:Terminal
    {
        public int Value { get;  set; }

        public Num() : base("Num")
        {            
        }

        public void SetValue(string value)
        {
            Value = int.Parse(value);
        }

        public override Symbol GetCopy(string str)
        {
            var num = new Num();
            num.SetValue(str);
            return num;
        }

        public override bool IsEqual(string str)
        {
            return str.All(char.IsNumber);
        }
    }
}
