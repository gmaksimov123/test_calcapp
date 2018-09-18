using System;
using test_calcapp.Functions.Abstract;

namespace test_calcapp.Functions
{
    class AbsFunction : Function
    {
        public override double Evaluate(ExecFunc function, string data, ref int from)
        {
            double arg = function(data, ref from, Parser.EndArg);
            return Math.Abs(arg);
        }
    }
}
