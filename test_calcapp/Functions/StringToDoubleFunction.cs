using System;
using test_calcapp.Functions.Abstract;

namespace test_calcapp.Functions
{
    class StringToDoubleFunction : Function
    {
        private readonly string _item;

        public StringToDoubleFunction(string item)
        {
            _item = item;
        }
        public override double Evaluate(ExecFunc function, string data, ref int from)
        {
            double num;
            if (!double.TryParse(_item, out num))
            {
                throw new ArgumentException("Could not parse token [" + _item + "]");
            }
            return num;
        }
        
    }
}
