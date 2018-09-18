using Microsoft.VisualStudio.TestTools.UnitTesting;
using test_calcapp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace test_calcapp.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void OperationsTest()
        {
            using (var parser = ParserFactory.CreateDefault())
            {
                Assert.AreEqual(parser.Process("1+2"), 1 + 2);
                Assert.AreEqual(parser.Process("1-2"), 1 - 2);
                Assert.AreEqual(parser.Process("1*2"), 1 * 2);
                Assert.AreEqual(parser.Process("1/2"), 1 / 2.0);
                Assert.AreEqual(parser.Process("-1"), -1);
                Assert.AreEqual(parser.Process("-1,6"), -1.6);

                Assert.AreEqual(parser.Process("(1-(2))"), (1 - (2)));
                Assert.AreEqual(parser.Process("3+2*6-1"), 3 + 2 * 6 - 1);
                Assert.AreEqual(parser.Process("3-2*6-1"), 3 - 2 * 6 - 1);
                Assert.AreEqual(parser.Process("1-2-3-(4-(5-(6-7)))"), 1 - 2 - 3 - (4 - (5 - (6 - 7))));
                Assert.AreEqual(parser.Process("3-(5-6)-(2-(3-(1-2)))"), 3 - (5 - 6) - (2 - (3 - (1 - 2))));
                Assert.AreEqual(parser.Process("3-(5-6)-(2-(3-(1+2)))+2-(-1+7)*(9-2)/((16-3)-3)+15/2*5"),
                  3 - (5 - 6) - (2 - (3 - (1 + 2))) + 2 - (-1 + 7) * (9 - 2) / ((16.0 - 3) - 3.0) + 15 / 2.0 * 5);
                Assert.AreEqual(parser.Process("(-1+7)*(9-2)"), (-1 + 7) * (9 - 2));
                Assert.AreEqual(parser.Process("((16-3)-3)+15/2*5"), ((16 - 3) - 3) + 15 / 2.0 * 5);
                Assert.AreEqual(parser.Process("1+15/2*5"), 1 + 15 / 2.0 * 5);
                Assert.AreEqual(parser.Process("3-2/6-1,8"), 3 - 2 / 6.0 - 1.8);
            }

        }

        [TestMethod()]
        public void FunctionsTest()
        {
            using (var parser = ParserFactory.CreateDefault())
            {
                Assert.AreEqual(parser.Process("abs(-1)"), Math.Abs(-1));
                Assert.AreEqual(parser.Process("abs(22)"), Math.Abs(22));
            }
        }

        [TestMethod()]
        public void DecimalPointTest()
        {
            using (var parser = ParserFactory.CreateDefault())
            {
                //decimal point depends on current culture; ',' for ru-RU
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                Assert.AreEqual(parser.Process("1/0,5"), 1 / 0.5);
                Assert.AreEqual(parser.Process("-5,5"), -5.5);

                //'.' for en-GB
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
                Assert.AreEqual(parser.Process("1/0.5"), 1 / 0.5);
                Assert.AreEqual(parser.Process("-5.5"), -5.5);
            }


        }
        [TestMethod()]
        public void ParenthesisTest()
        {
            using (var parser = ParserFactory.CreateDefault())
            {
                Assert.AreEqual(parser.Process("(10)"), (10));
                Assert.AreEqual(parser.Process("((10))"), ((10)));
                Assert.AreEqual(parser.Process("(((10)))"), (((10))));


                Assert.AreEqual(parser.Process("(-5)"), (-5));
                Assert.AreEqual(parser.Process("(-6.7)"), (-6.7));
            }


        }
    }
}