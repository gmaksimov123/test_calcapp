using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_calcapp.Functions;
using test_calcapp.Functions.Abstract;

namespace test_calcapp
{
    class FunctionsConfig : NinjectModule
    {
        public override void Load()
        {
            //need these functions for parsing
            Bind<Function>().To<IdentityFunction>().Named("Identity");
            Bind<Function>().To<StringToDoubleFunction>().Named("StringToDouble");

            //add any optional functions
            Bind<Function>().To<AbsFunction>().Named("abs");
        }
    }
}
