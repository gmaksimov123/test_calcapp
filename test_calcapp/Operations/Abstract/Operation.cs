namespace test_calcapp.Operations.Abstract
{
    public abstract class Operation
    {
        public Operation(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }

        public abstract double Execute(double operandLeft, double operandRight);
    }
}
