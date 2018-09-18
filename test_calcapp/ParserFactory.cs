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

    }
}
