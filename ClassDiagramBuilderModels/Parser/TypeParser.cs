﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        private readonly Regex typeInfoHeaderPattern = new Regex(@"");
        private readonly Regex fieldDeclarationPattern = new Regex(@"");
        private readonly Regex propertyDeclarationPattern = new Regex(@"");
        private readonly Regex methodDeclarationPattern = new Regex(@"");
        private readonly Regex constructorDeclarationPattern = new Regex(@"");

        private string currentNameSpace;
        private string currentFilename;

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
                var fileMemberHirarchy = GetFileMembersTree(Path.GetFileName(path), data);
                return fileMemberHirarchy;
            }
            else
            {
                Trace.WriteLine($"Invalid data: {path}");
                return null;
            }
        }

        public List<TypeInfo> GetTypeInfos(Node<string> FileMemberHirarchy)
        {
            ArgumentNullException.ThrowIfNull(FileMemberHirarchy);

            var typeInfos = new List<TypeInfo>();
            var filename = FileMemberHirarchy?.Header;
            var nameSpace = FileMemberHirarchy?.Children?.FirstOrDefault()?.Header;

            if (string.IsNullOrEmpty(filename))
            {
                throw new Exception($"{nameof(filename)} is null or empty!");
            }

            if (string.IsNullOrEmpty(nameSpace))
            {
                throw new Exception($"{nameof(nameSpace)} is null or empty!");
            }

            currentFilename = filename;
            currentNameSpace = nameSpace;

            foreach (var rawTypeInfo in FileMemberHirarchy.Children.FirstOrDefault().Children)
            {
                var typeInfo = GetTypeInfo(rawTypeInfo);
                if(typeInfo != null)
                {
                    typeInfos.Add(typeInfo);
                }
            }

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

        private TypeInfo GetTypeInfo(Node<string> rawTypeInfo)
        {
            if (string.IsNullOrEmpty(currentNameSpace))
            {
                throw new Exception($"{nameof(currentNameSpace)} is null or empty!");
            }

            foreach(var memberInfo in rawTypeInfo.Children)
            {

            }

            TypeInfo typeInfo = null;
            return typeInfo;
        }
    }
}
