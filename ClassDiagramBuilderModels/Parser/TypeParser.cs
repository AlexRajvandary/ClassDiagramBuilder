using ClassDiagramBuilder.Models.TypeAnalyzerModels;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        public Node<string>? Parse(string path)
        {
            string data;
            using (var sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }

            //data = Regex.Replace(data, @"\s+", " ");
            //data = data.Replace(Environment.NewLine, string.Empty);
            if (IsBracketsBalanced(data))
            {
                var tree = TreeFromString(data);
                return tree;
            }
            else
            {
                Trace.WriteLine($"Invalid data: {path}");
                return null;
            }
        }

        private bool IsBracketsBalanced(string str)
        {
            var stack = new Stack<int>();

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    stack.Push(i);
                }
                else if (str[i] == '}')
                {
                    if (!stack.TryPop(out _))
                    {
                        return false;
                    }
                }
            }

            return stack.Count == 0;
        }

        private Node<string> TreeFromString(string str)
        {
            var openBracketsIndexStack = new Stack<int>();
            var root = new Node<string>();

            var currentNode = root;

            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    openBracketsIndexStack.Push(i);
                    var childNode = new Node<string>();
                    currentNode.AddChild(childNode);
                    currentNode = childNode;
                }
                else if (str[i] == '}')
                {
                    if (!openBracketsIndexStack.TryPop(out var _))
                    {
                        Trace.WriteLine($"To many closed brackets: {i}");
                    }
                    else
                    {
                        currentNode = currentNode.Parent;
                    }
                }
                else
                {
                    currentNode.Data += str[i];
                }
            }

            return root;
        }
    }

    public class Token
    {
        public Token()
        {

        }

        public Token(string value)
        {
            Value = value;
        }

        public Token(string name, string value)
        {
            Header = name;
            Value = value;
        }

        public string Header { get; set; }

        public string Value { get; set; }
    }
}
