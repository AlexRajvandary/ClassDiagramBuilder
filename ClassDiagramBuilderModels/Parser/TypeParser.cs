using System.Diagnostics;

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
            data = data.Replace("\r\n", string.Empty);
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
                        if (i + 1 < str.Length)
                        {
                            currentNode = currentNode.Parent.Parent;
                            if(str[i + 1] != '}')
                            {
                                var newChild = new Node<string>();
                                currentNode.AddChild(newChild);
                                currentNode = newChild;
                            }
                        }
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
}
