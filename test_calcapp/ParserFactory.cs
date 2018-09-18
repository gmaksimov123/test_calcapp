using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_calcapp
{
    public static class ParserFactory
    {
        public static Parser CreateDefault()
        {
            return new Parser(
                new OperationsConfig(),
                new FunctionsConfig()
                );
        }
        public static Parser CreateWithModules(params NinjectModule[] modules)
        {
            return new Parser(modules);
        }
    }
}
