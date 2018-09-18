using System;
using test_calcapp.Operations.Abstract;
namespace test_calcapp.Operations
{
    public class Multiply : Operation
    {
        public Multiply(int priority) : base(priority)
        {
        }

        public override double Execute(double operandLeft, double operandRight)
        {
            return operandLeft * operandRight;
        }
    }
}