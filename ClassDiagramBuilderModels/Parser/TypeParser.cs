using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

            data = Regex.Replace(data, @"\s+", " ");
            if (IsBracketsBalanced(data))
            {
                var fileMemberHirarchy = GetFileMembersTree(path, data);
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

        private Node<string> GetFileMembersTree(string filename, string fileRawContent)
        {
            var openBracketsIndexStack = new Stack<int>();
            var root = new Node<string>();
            root.Header = filename;
            var newChildNode = new Node<string>();
            root.AddChild(newChildNode);
            var currentNode = newChildNode;

            for (var charIndex = 0; charIndex < fileRawContent.Length; charIndex++)
            {
                var previousChar = charIndex - 1 > 0 ? fileRawContent[charIndex - 1] : char.MinValue;
                var currentChar = fileRawContent[charIndex];
                var nextChar = charIndex + 1 < fileRawContent.Length ? fileRawContent[charIndex + 1] : char.MinValue;

                if (currentChar == '{')
                {
                    openBracketsIndexStack.Push(charIndex);

                    var childNode = new Node<string>();
                    currentNode.AddChild(childNode);
                    currentNode = childNode;
                }
                else if (currentChar == '}')
                {
                    if (!openBracketsIndexStack.TryPop(out var _))
                    {
                        Trace.WriteLine($"To many closed brackets: {charIndex}");
                        return null;
                    }
                    else
                    {
                        var parent = currentNode.Parent;
                        if (string.IsNullOrEmpty(currentNode.Header))
                        {
                            parent.RemoveChild(currentNode);
                        }

                        currentNode = parent;
                    }
                }
                else if (currentChar == ';')
                {
                    if (currentNode.Parent != null)
                    {
                        currentNode = currentNode.Parent;
                    }
                }
                else
                {
                    if (previousChar == ';' && nextChar != '}' || previousChar == '}')
                    {
                        var child = new Node<string>();
                        currentNode.AddChild(child);
                        currentNode = child;
                    }
                    else
                    {
                        if (!currentNode.HeaderReadOnly)
                        {
                            currentNode.Header += fileRawContent[charIndex];
                        }
                    }
                }
            }

            return root;
        }
    }
}
