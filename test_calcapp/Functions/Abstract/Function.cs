namespace test_calcapp.Functions.Abstract
{
    public abstract class Function
    {

        public delegate double ExecFunc(string data, ref int from, char to);

        public abstract double Evaluate(ExecFunc function, string data, ref int from);
    }
}
