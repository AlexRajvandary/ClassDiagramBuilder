using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        private readonly Regex typeInfoHeaderPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsAbstract>abstract)?\s?(?<IsSealed>sealed)?\s?(?<IsStatic>static)?\s?(?<TypeKind>class|struct|interface){1}\s?(?<TypeName>[A-Za-z]+)\s?(?<Generic><(?<GenericType>.+)>)?\s?(?<IsDerrived>:\s?((?<Derrived>\w+)\s?)+)?\z");
        private readonly Regex fieldDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<IsReadOnly>readonly)?\s?(?<IsConstant>constant)?\s?(?<TypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>\w+)(?<HasValue>\s?=(?<Value>.+))?\z");
        private readonly Regex propertyDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?\s?(?<TypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>\w+)\z");
        private readonly Regex methodDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<IsReadOnly>readonly)?\s?(?<IsConstant>constant)?\s?(?<ReturnTypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>\w+)\s?(?<Params>\((?<Parameters>.)\))\z");
        private readonly Regex constructorDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<IsReadOnly>readonly)?\s?(?<IsConstant>constant)?\s?(?<ReturnTypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)\s?(?<Params>\((?<Parameters>.)\))\z");

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
            var nameSpace = FileMemberHirarchy?.Children?.LastOrDefault()?.Header;

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

            if (FileMemberHirarchy?.Children?.LastOrDefault()?.Children?.Any() ?? false)
            {
                foreach (var rawTypeInfo in FileMemberHirarchy.Children.LastOrDefault().Children)
                {
                    var typeInfo = GetTypeInfo(rawTypeInfo);
                    if (typeInfo != null)
                    {
                        typeInfos.Add(typeInfo);
                    }
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

            var data = rawTypeInfo?.Header?.Trim();

            if (string.IsNullOrWhiteSpace(data) || !typeInfoHeaderPattern.IsMatch(data))
            {
                throw new Exception($"{data} is invalid.");
            }

            var typeInfoHeader = typeInfoHeaderPattern.Match(data);

            var acsessModifier = typeInfoHeader.Groups["AccsessModifier"].Success
                ? Enum.Parse<AcsessModifiers>(typeInfoHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                : AcsessModifiers.Private;

            var typeKind = Enum.Parse<TypeKind>(typeInfoHeader.Groups["TypeKind"].Value, ignoreCase: true);
            var typeInfoName = typeInfoHeader.Groups["TypeName"].Value;
            var isAbstract = typeInfoHeader.Groups["IsAbstract"].Success;
            var isSealed = typeInfoHeader.Groups["IsSealed"].Success;
            var isStatic = typeInfoHeader.Groups["IsStatic"].Success;

            var fields = new List<FieldInfo>();
            var properties = new List<PropertyInfo>();
            var methods = new List<MethodInfo>();
            var constructors = new List<ConstructorInfo>();

            if (rawTypeInfo.Children != null)
            {
                foreach (var memberInfo in rawTypeInfo.Children)
                {
                    data = memberInfo?.Header?.Trim();

                    if (string.IsNullOrWhiteSpace(data))
                    {
                        continue;
                    }

                    if (fieldDeclarationPattern.IsMatch(data))
                    {
                        var fieldHeader = fieldDeclarationPattern.Match(data);

                        var fieldAcsessModifier = fieldHeader.Groups["AccsessModifier"].Success
                            ? Enum.Parse<AcsessModifiers>(fieldHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                            : AcsessModifiers.Private;

                        var fieldName = fieldHeader.Groups["Name"].Value;
                        var typeName = fieldHeader.Groups["TypeName"].Value;
                        var isFiledAbstract = fieldHeader.Groups["IsAbstract"].Success;
                        var isFieldStatic = fieldHeader.Groups["IsStatic"].Success;
                        var isFieldReadOnly = fieldHeader.Groups["IsReadOnly"].Success;
                        var isFieldConstant = fieldHeader.Groups["IsConstant"].Success;
                        var fieldValue = fieldHeader.Groups["Value"].Value;

                        fields.Add(new FieldInfo(fieldAcsessModifier,
                                                 isAbstract,
                                                 isFieldConstant,
                                                 isStatic,
                                                 isFieldReadOnly,
                                                 fieldName,
                                                 currentNameSpace,
                                                 typeName));
                    }
                    else if (propertyDeclarationPattern.IsMatch(data))
                    {
                        var propertyHeader = propertyDeclarationPattern.Match(data);
                        acsessModifier = propertyHeader.Groups["AccsessModifier"].Success
                          ? Enum.Parse<AcsessModifiers>(propertyHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                          : AcsessModifiers.Private;

                        var propertyName = propertyHeader.Groups["Name"].Value;
                        var typeName = propertyHeader.Groups["TypeName"].Value;
                        var isPropertyAbstract = propertyHeader.Groups["IsAbstract"].Success;
                        var isPropertyStatic = propertyHeader.Groups["IsStatic"].Success;

                        properties.Add(new PropertyInfo(acsessModifier,
                                                        isPropertyAbstract,
                                                        isAuto: true,
                                                        isPropertyStatic,
                                                        typeName,
                                                        propertyName,
                                                        currentNameSpace));
                    }
                    else if (methodDeclarationPattern.IsMatch(data))
                    {
                        List<TypeInfo> parameters = new List<TypeInfo>();

                        var methodHeader = methodDeclarationPattern.Match(data);
                        var methodAcsessModifier = methodHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(methodHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var methodName = methodHeader.Groups["Name"].Value;
                        var methodReturnType = methodHeader.Groups["TypeName"].Value;
                        var isMethodAbstract = methodHeader.Groups["IsAbstract"].Success;
                        var isMethodStatic = methodHeader.Groups["IsStatic"].Success;

                        methods.Add(new MethodInfo(methodAcsessModifier,
                                                   isMethodAbstract,
                                                   isMethodStatic,
                                                   methodName,
                                                   currentNameSpace,
                                                   parameters,
                                                   methodReturnType));
                    }
                    else if (constructorDeclarationPattern.IsMatch(data))
                    {
                        List<TypeInfo> parameters = new List<TypeInfo>();

                        var constructorHeader = constructorDeclarationPattern.Match(data);
                        var constructorAcsessModifier = constructorHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(constructorHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var isConstructorAbstract = constructorHeader.Groups["IsAbstract"].Success;
                        var isConstructorStatic = constructorHeader.Groups["IsStatic"].Success;

                        constructors.Add(new ConstructorInfo(constructorAcsessModifier,
                                                             isConstructorStatic,
                                                             currentNameSpace,
                                                             parameters));
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            var typeInfo = new TypeInfo(acsessModifier,
                                        constructors,
                                        fields,
                                        isAbstract,
                                        isStatic,
                                        methods,
                                        typeInfoName,
                                        currentNameSpace,
                                        properties,
                                        typeKind);

            return typeInfo;
        }
    }
}
