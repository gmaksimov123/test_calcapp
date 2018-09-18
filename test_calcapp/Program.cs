using System;

namespace test_calcapp
{
    class Program
    {
        static void Calculate(Parser parser, string expr, double expected)
        {
            double result = parser.Process(expr);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            string outcome =  result == expected ? "OK" : "NOK " + expected;
            Console.WriteLine("{0} --> {1} ({2})", expr, result, outcome);
        }

        static void Main()
        {
            using (var parser = ParserFactory.CreateDefault())
            {
                Calculate(parser, "1-2", 1 - 2);
                Calculate(parser, "(((-5,5)))", (((-5.5))));
                Calculate(parser, "(1-(2))", (1 - (2)));
                Calculate(parser, "3+2*6-1", 3 + 2 * 6 - 1);
                Calculate(parser, "3-2*6-1", 3 - 2 * 6 - 1);
                Calculate(parser, "1-2-3-(4-(5-(6-7)))", 1 - 2 - 3 - (4 - (5 - (6 - 7))));
                Calculate(parser, "3-(5-6)-(2-(3-(1-2)))", 3 - (5 - 6) - (2 - (3 - (1 - 2))));
                Calculate(parser, "3-(5-6)-(2-(3-(1+2)))+2-(-1+7)*(9-2)/((16-3)-3)+15/2*5",
                  3 - (5 - 6) - (2 - (3 - (1 + 2))) + 2 - (-1 + 7) * (9 - 2) / ((16.0 - 3) - 3.0) + 15 / 2.0 * 5);
                Calculate(parser, "(-1+7)*(9-2)", (-1 + 7) * (9 - 2));
                Calculate(parser, "((16-3)-3)+15/2*5", ((16 - 3) - 3) + 15 / 2.0 * 5);
                Calculate(parser, "1+15/2*5", 1 + 15 / 2.0 * 5);
                Calculate(parser, "3-2/6-1", 3 - 2 / 6.0 - 1);
            }

            Console.WriteLine();
            Console.ReadKey();

        }
    }
}
