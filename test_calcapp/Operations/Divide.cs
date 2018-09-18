using System;
using test_calcapp.Operations.Abstract;

namespace test_calcapp.Operations
{
    public class Divide : Operation
    {
        public Divide(int priority) : base(priority)
        {

        }

        public override double Execute(double operandLeft, double operandRight)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if ( operandRight == 0 )
            {
                throw new ArgumentException("Division by zero");
            }
            return operandLeft / operandRight;
        }
    }
}