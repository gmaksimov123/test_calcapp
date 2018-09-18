using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_calcapp.Operations.Abstract;
using test_calcapp.Functions.Abstract;
using Ninject.Parameters;
using Ninject.Modules;

namespace test_calcapp
{
    public class Parser : IDisposable
    {
        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char END_LINE = '\n';

        protected IKernel kernel;

        public Parser(params NinjectModule[] modules)
        {
            kernel = new StandardKernel(modules);
        }

        protected class Cell
        {
            internal Cell(double value, char action, Operation action_new)
            {
                Value = value;
                Action_old = action;
                Action = action_new;
            }

            internal double Value { get; set; }
            internal char Action_old { get; set; }
            internal Operation Action { get; set; }
        }

        public double Process(string data)
        {
            // Get rid of spaces and check parenthesis
            var expression = Preprocess(data);
            var from = 0;

            return LoadAndCalculate(data, ref from, END_LINE);
        }

        protected string Preprocess(string data)
        {
            if ( string.IsNullOrEmpty(data) )
            {
                throw new ArgumentException("Loaded empty data");
            }

            var parentheses = 0;
            var result = new StringBuilder(data.Length);

            for (var i = 0; i < data.Length; i++ )
            {
                var ch = data[i];
                switch ( ch )
                {
                    case ' ':
                    case '\t':
                    case '\n': continue;
                    case END_ARG:
                        parentheses--;
                        break;
                    case START_ARG:
                        parentheses++;
                        break;
                }
                result.Append(ch);
            }

            if ( parentheses != 0 )
            {
                throw new ArgumentException("Uneven parenthesis");
            }

            return result.ToString();
        }

        protected double LoadAndCalculate(string data, ref int from, char to = END_LINE)
        {
            if ( from >= data.Length || data[from] == to )
            {
                throw new ArgumentException("Loaded invalid data: " + data);
            }

            var listToMerge = new List<Cell>(16);
            var item = new StringBuilder();

            do
            { // Main processing cycle of the first part.
                var ch = data[from++];
                if ( StillCollecting(item.ToString(), ch, to) )
                { // The char still belongs to the previous operand.
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

                var action = ValidAction(ch) ? ch
                                              : UpdateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(value, action, kernel.TryGet<Operation>(action.ToString())));
                item.Clear();

            } while ( from < data.Length && data[from] != to );

            if ( from < data.Length &&
               (data[from] == END_ARG || data[from] == to) )
            { // This happens when called recursively: move one char forward.
                from++;
            }

            var baseCell = listToMerge[0];
            var index = 1;

            return Merge(baseCell, ref index, listToMerge);
        }

        protected bool StillCollecting(string item, char ch, char to)
        {
            // Stop collecting if either got END_ARG ')' or to char, e.g. ','.
            var stopCollecting = (to == END_ARG || to == END_LINE) ?
                                   END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                  !(ValidAction(ch) || ch == START_ARG || ch == stopCollecting);
        }

        protected bool ValidAction(char ch)
        {
            return kernel.TryGet<Operation>(ch.ToString()) != null;
        }

        protected char UpdateAction(string item, ref int from, char ch, char to)
        {
            if ( from >= item.Length || item[from] == END_ARG || item[from] == to )
            {
                return END_ARG;
            }

            var index = from;
            var res = ch;
            while ( !ValidAction(res) && index < item.Length )
            { // Look for the next character in string until a valid action is found.
                res = item[index++];
            }

            from = ValidAction(res) ? index
                                    : index > from ? index - 1
                                                   : from;
            return res;
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
            if (item.Length == 0 && ch == START_ARG)
            {
                // There is no function, just an expression in parentheses
                return kernel.Get<Function>("Identity");
            }
            var m_impl = kernel.TryGet<Function>(item);

            if (m_impl != null)
            {
                // Function exists and is registered (e.g. pi, exp, etc.)
                return m_impl;
            }

            // Function not found, will try to parse this as a number.
            return kernel.Get<Function>("StringToDouble", new ConstructorArgument("item", item));
        }

        public void Dispose()
        {
            kernel.Dispose();
        }
    }

}