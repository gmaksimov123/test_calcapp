using test_calcapp.Operations.Abstract;
using test_calcapp.Operations;
using Ninject.Modules;

namespace test_calcapp
{
    public class OperationsConfig : NinjectModule
    {
        public override void Load()
        {
            //operation can be any single character symbol
            Bind<Operation>().To<Plus>().Named("+").WithConstructorArgument("priority", 2);
            Bind<Operation>().To<Minus>().Named("-").WithConstructorArgument("priority", 2);
            Bind<Operation>().To<Divide>().Named("/").WithConstructorArgument("priority", 3);
            Bind<Operation>().To<Multiply>().Named("*").WithConstructorArgument("priority", 3);
        }
    }
}
