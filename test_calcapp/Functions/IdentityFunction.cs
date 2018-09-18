using test_calcapp.Functions.Abstract;

namespace test_calcapp.Functions
{
    class IdentityFunction : Function
    {
        public override double Evaluate(ExecFunc function, string data, ref int from)
        {
            return function(data, ref from, Parser.EndArg);
        }
    }

}
