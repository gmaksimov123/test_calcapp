using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_calcapp.Functions.Abstract;

namespace test_calcapp.Functions
{
    class AbsFunction : Function
    {
        public override double Evaluate(ExecFunc<string, int, char, double> function, string data, ref int from)
        {
            double arg = function(data, ref from, Parser.END_ARG);
            return Math.Abs(arg);
        }
    }
}
