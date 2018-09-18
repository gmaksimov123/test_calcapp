using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_calcapp.Functions.Abstract
{
    public abstract class Function
    {

        public delegate double ExecFunc<in String, in Int, Char, out Double>(string data, ref int from, char to);

        public abstract double Evaluate(ExecFunc<string, int, char, double> function, string data, ref int from);
    }
}
