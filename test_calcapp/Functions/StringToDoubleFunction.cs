using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_calcapp.Functions.Abstract;

namespace test_calcapp.Functions
{
    class StringToDoubleFunction : Function
    {
        public StringToDoubleFunction(string item)
        {
            Item = item;
        }
        public override double Evaluate(ExecFunc<string, int, char, double> function, string data, ref int from)
        {
            if (!double.TryParse(Item, out double num))
            {
                throw new ArgumentException("Could not parse token [" + Item + "]");
            }
            return num;
        }
        public string Item { private get; set; }
    }
}
