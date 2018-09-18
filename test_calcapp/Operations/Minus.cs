using test_calcapp.Operations.Abstract;
namespace test_calcapp.Operations
{
    public class Minus : Operation
    {
        public Minus(int priority) : base(priority)
        {
        }

        public override double Execute(double operandLeft, double operandRight)
        {
            return operandLeft - operandRight;
        }
    }
}