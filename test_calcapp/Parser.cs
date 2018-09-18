using Ninject;
using System;
using System.Collections.Generic;
using System.Text;
using test_calcapp.Operations.Abstract;
using test_calcapp.Functions.Abstract;
using Ninject.Parameters;
using Ninject.Modules;

namespace test_calcapp
{
    public class Parser : IDisposable
    {
        public const char StartArg = '(';
        public const char EndArg = ')';
        public const char EndLine = '\n';

        private readonly IKernel _kernel;

        public Parser(params INinjectModule[] modules)
        {
            _kernel = new StandardKernel(modules);
        }

        protected class Cell
        {
            internal Cell(double value, Operation action)
            {
                Value = value;
                Action = action;
            }

            internal double Value { get; set; }
            internal Operation Action { get; set; }
        }

        public double Process(string data)
        {
            // Get rid of spaces and check parenthesis
            Preprocess(data);
            var from = 0;

            return LoadAndCalculate(data, ref from);
        }

        protected void Preprocess(string data)
        {
            if ( string.IsNullOrEmpty(data) )
            {
                throw new ArgumentException("Loaded empty data");
            }

            var parentheses = 0;
            var result = new StringBuilder(data.Length);

            foreach (var ch in data)
            {
                if (ch == ' ' || ch == '\t' || ch == '\n')
                {
                    continue;
                }

                if (ch == EndArg)
                {
                    parentheses--;

                }
                else if ( ch == StartArg )
                {
                     parentheses++;
                }

                result.Append(ch);
            }

            if ( parentheses != 0 )
            {
                throw new ArgumentException("Uneven parenthesis");
            }
        }

        protected double LoadAndCalculate(string data, ref int from, char to = EndLine)
        {
            if ( from >= data.Length || data[from] == to )
            {
                throw new ArgumentException("Loaded invalid data: " + data);
            }

            var listToMerge = new List<Cell>(16);
            var item = new StringBuilder();

            while (from < data.Length && data[from] != to)
            {
                var ch = data[from++];

                var stopCollecting = to == EndArg || to == EndLine ? EndArg : to;

                var stillCollecting = item.Length == 0 && (ch == '-' || ch == EndArg) ||
                       !(ValidAction(ch) || ch == StartArg || ch == stopCollecting);

                if ( stillCollecting )
                { 
                    // The char still belongs to the previous operand.
                    item.Append(ch);
                    if ( from < data.Length && data[from] != to )
                    {
                        continue;
                    }
                }

                // We are done getting the next token. The Evaluate() call below may
                // recursively call LoadAndCalculate(). This will happen if extracted
                // item is a function or if the next item is starting with a START_ARG '('.
                var func = GetFunction(item.ToString(), ch);
                var value = func.Evaluate(LoadAndCalculate, data, ref from);

                var operation = GetOperation(ch) ?? UpdateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(value, operation));
                item.Clear();
            }

            if ( from < data.Length && (data[from] == EndArg || data[from] == to) )
            { 
                // This happens when called recursively: move one char forward.
                from++;
            }

            var baseCell = listToMerge[0];
            var index = 1;

            return Merge(baseCell, ref index, listToMerge);
        }


        protected bool ValidAction(char ch)
        {
            return GetOperation(ch) != null;
        }

        protected Operation GetOperation(char ch)
        {
            return _kernel.TryGet<Operation>(ch.ToString());
        }

        protected Operation UpdateAction(string item, ref int from, char ch, char to)
        {
            if ( from >= item.Length || item[from] == EndArg || item[from] == to )
            {
                return null;
            }

            var index = from;
            var res = ch;

            var operation = GetOperation(res);

            while ( operation == null && index < item.Length )
            { 
                // Look for the next character in string until a valid action is found.
                res = item[index++];
                operation = GetOperation(res);
            }

            if ( operation != null)
            {
                from = index;
            }
            else
            {
                from = index > from ? index - 1 : from;
            }
            return operation;
        }

        // From outside this function is called with mergeOneOnly = false.
        // It also calls itself recursively with mergeOneOnly = true, meaning
        // that it will return after only one merge.
        protected double Merge(Cell current, ref int index, List<Cell> listToMerge,
                     bool mergeOneOnly = false)
        {
            while ( index < listToMerge.Count )
            {
                var next = listToMerge[index++];

                while ( !CanMergeCells(current, next) )
                { // If we cannot merge cells yet, go to the next cell and merge
                  // next cells first. E.g. if we have 1+2*3, we first merge next
                  // cells, i.e. 2*3, getting 6, and then we can merge 1+6.
                    Merge(next, ref index, listToMerge, true /* mergeOneOnly */);
                }
                MergeCells(current, next);
                if ( mergeOneOnly )
                {
                    return current.Value;
                }
            }

            return current.Value;
        }

        protected void MergeCells(Cell leftCell, Cell rightCell)
        {

            if ( leftCell.Action != null )
            {
                leftCell.Value = leftCell.Action.Execute(leftCell.Value, rightCell.Value);
            }

            leftCell.Action = rightCell.Action;           
        }

        protected bool CanMergeCells(Cell leftCell, Cell rightCell)
        {
            return GetPriority(leftCell.Action) >= GetPriority(rightCell.Action);
        }

        protected int GetPriority(Operation action)
        {
            return action?.Priority ?? 0;
        }
        protected Function GetFunction(string item, char ch)
        {
            if (item.Length == 0 && ch == StartArg)
            {
                // There is no function, just an expression in parentheses
                return _kernel.Get<Function>("Identity");
            }
            var mImpl = _kernel.TryGet<Function>(item);

            // ?? Function not found, will try to parse this as a number.
            return mImpl ?? _kernel.Get<Function>("StringToDouble", new ConstructorArgument("item", item));
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }
    }

}