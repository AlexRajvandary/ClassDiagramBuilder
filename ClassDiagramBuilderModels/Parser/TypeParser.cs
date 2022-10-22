using System.Diagnostics;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        public Node<string>? GetFileMemberHirarchy(string path)
        {
            string data;
            using (var sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }
            data = data.Replace("\r\n", string.Empty);
            if (IsBracketsBalanced(data))
            {
                var fileMemberHirarchy = GetFileMembersTree(data);
                return fileMemberHirarchy;
            }
            else
            {
                Trace.WriteLine($"Invalid data: {path}");
                return null;
            }
        }

        public List<TypeInfo> GetTypesInfo(Node<List<MemberParser>> fileMembersTree)
        {
            var typeInfos = new List<TypeInfo>();
            //if (fileMembersTree.Data.Last().Level == TokenLevel.Root)
            //{
            //    foreach (var fileMember in fileMembersTree.Children)
            //    {
            //        foreach (var typeDataItem in fileMember.Data)
            //        {
            //            //if (typeDataItem.Level != TokenLevel.Namespace) return null;

            //            //var typekind = typeDataItem.TokenType switch
            //            //{
            //            //    TokenType.Struct => TypeKind.Struct,
            //            //    TokenType.Class => TypeKind.Class,
            //            //    TokenType.Enum => TypeKind.Enum,
            //            //    _ => TypeKind.Undefined
            //            //};

            //            //var accsessModifier = typeDataItem.AcsessModifier;
            //            //var ctors = typeDataItem.GetCtors();
            //            //var methods = typeDataItem.GetMethods();
            //            //var properies = typeDataItem.GetProperties();
            //            //var fields = typeDataItem.GetFields();
            //            //var name = typeDataItem.Header;

            //            //typeInfos.Add(new TypeInfo(accsessModifier,
            //            //                           ctors,
            //            //                           fields,
            //            //                           false,
            //            //                           methods,
            //            //                           name,
            //            //                           "namespace",
            //            //                           properies,
            //            //                           typekind));
            //        }
            //    }
            //}

            return typeInfos;
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

        private Node<string> GetFileMembersTree(string str)
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
                            if (str[i + 1] != '}')
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
