using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        private readonly Regex typeInfoHeaderPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsAbstract>abstract)?\s?(?<IsStatic>static)?\s?(?<IsSealed>sealed)?\s?(?<IsPartial>partial)?\s?(?<TypeKind>class|struct|interface|enum){1}\s?(?<TypeName>[A-Za-z0-9]+)\s?(?<Generic><(?<GenericType>[A-Za-z0-9]+)>)?\s?(?<IsDerrived>:\s?((?<Derrived>\w+)\s?)+)?\z");
        private readonly Regex fieldDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<IsReadOnly>readonly)?\s?(?<IsConstant>constant)?\s?(?<TypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>[A-Za-z0-9]+)(?<HasValue>\s?=(?<Value>.+))?\z");
        private readonly Regex propertyDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<TypeName>\w+)?\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>\w+)\z");
        private readonly Regex methodDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<ReturnTypeName>\w+){1}\s?(?<IsGeneric><(?<GenericType>.+)>)?\s(?<Name>\w+){1}\s(?<Params>\((.+)\))\z");
        private readonly Regex constructorDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?(?<IsStatic>static)?\s?(?<ReturnTypeName>\w+){1}\s?(?<Params>\((?<Parameters>.+)\))\z");
        private readonly Regex eventDeclarationPattern = new Regex(@"^(?<AccsessModifier>public|private|internal|protected|protected\sinternal)?\s?event{1}\s(?<DelegateType>\w+)\s(?<Name>\w+)\z");
        private readonly Regex commentPattern = new Regex(@"^(\/\/\/|\/\/|\/\*)\s?(?<OpenXMLComment><summary>)?\s?(?<CommentContent>.*)?\s?(?<CloseXMLComment></summary>)?\s?\z");

        private string currentNameSpace;
        private string currentFilename;

        public Node<string>? GetFileMemberHirarchy(string path, int parseDepth = 5)
        {
            string data;
            using (var sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }

            data = Regex.Replace(data, @"\s+", " ");
            if (AreBracketsBalanced(data))
            {
                var fileMemberHirarchy = GetFileMembersTree(Path.GetFileName(path), data, parseDepth);
                return fileMemberHirarchy;
            }
            else
            {
                Trace.WriteLine($"Invalid data: {path}");
                return null;
            }
        }

        public async Task<Node<string>?> GetFileMemberHirarchyAsync(string path, int parseDepth = 5)
        {
            string data;
            using (var sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }

            data = Regex.Replace(data, @"\s+", " ");
            if (AreBracketsBalanced(data))
            {
                var fileMemberHirarchy = await GetFileMembersTreeAsync(Path.GetFileName(path), data, parseDepth);
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
                    var typeInfo = BuildTypeInfo(rawTypeInfo);
                    if (typeInfo != null)
                    {
                        typeInfos.Add(typeInfo);
                    }
                }
            }

            return typeInfos;
        }

        public async Task<List<TypeInfo>> GetTypeInfosAsync(Node<string> FileMemberHirarchy)
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
                var rawTypeInfos = FileMemberHirarchy.Children.LastOrDefault().Children;
                foreach (var rawTypeInfo in rawTypeInfos)
                {
                    var typeInfo = await BuildTypeInfoAsync(rawTypeInfo);
                    if (typeInfo != null)
                    {
                        typeInfos.Add(typeInfo);
                    }
                }
            }

            return typeInfos;
        }

        private bool AreBracketsBalanced(string str)
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

        private Node<string> GetFileMembersTree(string filename, string fileRawContent, int parseDepth)
        {
            var insideBlockBracketsStack = new Stack<char>();
            var root = new Node<string>();
            root.Header = filename;
            var newChildNode = new Node<string>();
            root.AddChild(newChildNode);
            var currentNode = newChildNode;

            var previousCharacter = '\0';
            var previousPreviousCharacter = '\0';

            foreach (var currentCharacter in fileRawContent)
            {
                if (currentCharacter != '}' && currentCharacter != ';' &&
                    (currentCharacter == '{'
                    || currentCharacter != ' ' && (previousCharacter == '}' || previousCharacter == ';')
                    || previousCharacter == ' ' && (previousPreviousCharacter == '}' || previousPreviousCharacter == ';') && currentCharacter != ' '))
                {
                    if (currentNode.Level < parseDepth)
                    {
                        var child = new Node<string>();
                        currentNode.AddChild(child);
                        currentNode = child;

                        if (currentCharacter != '{' && currentCharacter != '}' && currentCharacter != ';' && currentCharacter != ' ')
                        {
                            currentNode.Header += currentCharacter;
                        }
                    }
                    else
                    {
                        if(currentCharacter == '{')
                        {
                            insideBlockBracketsStack.Push(currentCharacter);
                        }
                        
                        currentNode.Header += currentCharacter;
                    }
                }
                else if (currentCharacter == '}' || currentCharacter == ';')
                {
                    if (currentNode.Level < parseDepth)
                    {
                        var childToRemove = currentNode;
                        currentNode = currentNode.Parent;

                        if (string.IsNullOrWhiteSpace(childToRemove.Header))
                        {
                            currentNode.RemoveChild(childToRemove);
                        }
                    }
                    else
                    {
                        if(currentCharacter != '}')
                        {
                            currentNode.Header += currentCharacter;
                            continue;
                        }

                        if (!insideBlockBracketsStack.TryPeek(out _))
                        {
                            currentNode.Header += currentCharacter;

                            var childToRemove = currentNode;
                            currentNode = currentNode.Parent;

                            if (string.IsNullOrWhiteSpace(childToRemove.Header))
                            {
                                currentNode.RemoveChild(childToRemove);
                            }

                            currentNode = currentNode.Parent;
                        }
                        else
                        {
                            insideBlockBracketsStack.Pop();
                            currentNode.Header += currentCharacter;
                        }
                    }
                }
                else
                {
                    if (currentCharacter == ' ' && (previousCharacter == ';' || previousCharacter == '}'))
                    {
                        continue;
                    }

                    currentNode.Header += currentCharacter;
                }

                previousPreviousCharacter = previousCharacter;
                previousCharacter = currentCharacter;
            }

            return root;
        }

        private async Task<Node<string>> GetFileMembersTreeAsync(string filename, string fileRawContent, int parseDepth)
        {
            var insideBlockBracketsStack = new Stack<char>();
            var root = new Node<string>();
            root.Header = filename;
            var newChildNode = new Node<string>();
            root.AddChild(newChildNode);
            var currentNode = newChildNode;

            var previousCharacter = '\0';
            var previousPreviousCharacter = '\0';

            foreach (var currentCharacter in fileRawContent)
            {
                if (currentCharacter != '}' && currentCharacter != ';' &&
                    (currentCharacter == '{'
                    || currentCharacter != ' ' && (previousCharacter == '}' || previousCharacter == ';')
                    || previousCharacter == ' ' && (previousPreviousCharacter == '}' || previousPreviousCharacter == ';') && currentCharacter != ' '))
                {
                    if (currentNode.Level < parseDepth)
                    {
                        var child = new Node<string>();
                        currentNode.AddChild(child);
                        currentNode = child;

                        if (currentCharacter != '{' && currentCharacter != '}' && currentCharacter != ';' && currentCharacter != ' ')
                        {
                            currentNode.Header += currentCharacter;
                        }
                    }
                    else
                    {
                        if (currentCharacter == '{')
                        {
                            insideBlockBracketsStack.Push(currentCharacter);
                        }

                        currentNode.Header += currentCharacter;
                    }
                }
                else if (currentCharacter == '}' || currentCharacter == ';')
                {
                    if (currentNode.Level < parseDepth)
                    {
                        var childToRemove = currentNode;
                        currentNode = currentNode.Parent;

                        if (string.IsNullOrWhiteSpace(childToRemove.Header))
                        {
                            currentNode.RemoveChild(childToRemove);
                        }
                    }
                    else
                    {
                        if (currentCharacter != '}')
                        {
                            currentNode.Header += currentCharacter;
                            continue;
                        }

                        if (!insideBlockBracketsStack.TryPeek(out _))
                        {
                            currentNode.Header += currentCharacter;

                            var childToRemove = currentNode;
                            currentNode = currentNode.Parent;

                            if (string.IsNullOrWhiteSpace(childToRemove.Header))
                            {
                                currentNode.RemoveChild(childToRemove);
                            }

                            currentNode = currentNode.Parent;
                        }
                        else
                        {
                            insideBlockBracketsStack.Pop();
                            currentNode.Header += currentCharacter;
                        }
                    }
                }
                else
                {
                    if (currentCharacter == ' ' && (previousCharacter == ';' || previousCharacter == '}'))
                    {
                        continue;
                    }

                    currentNode.Header += currentCharacter;
                }

                previousPreviousCharacter = previousCharacter;
                previousCharacter = currentCharacter;
            }

            return root;
        }

        private TypeInfo BuildTypeInfo(Node<string> rawTypeInfo)
        {
            if (string.IsNullOrEmpty(currentNameSpace))
            {
                throw new Exception($"{nameof(currentNameSpace)} is null or empty!");
            }

            var data = rawTypeInfo?.Header?.Trim();

            if (string.IsNullOrWhiteSpace(data) || !typeInfoHeaderPattern.IsMatch(data))
            {
                return null;
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
            var events = new List<EventInfo>();

            if (rawTypeInfo.Children != null)
            {
                foreach (var memberInfo in rawTypeInfo.Children)
                {
                    data = memberInfo?.Header?.Trim();

                    if (string.IsNullOrWhiteSpace(data))
                    {
                        continue;
                    }

                    if (!memberInfo.Children?.Any() ?? true && fieldDeclarationPattern.IsMatch(data))
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
                        var parameters = new List<TypeInfo>();

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
                        var parameters = new List<TypeInfo>();

                        var constructorHeader = constructorDeclarationPattern.Match(data);
                        var constructorAcsessModifier = constructorHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(constructorHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var isConstructorAbstract = constructorHeader.Groups["IsAbstract"].Success;
                        var isConstructorStatic = constructorHeader.Groups["IsStatic"].Success;
                        var returnTypeName = constructorHeader.Groups["ReturnTypeName"].Value;

                        constructors.Add(new ConstructorInfo(constructorAcsessModifier,
                                                             isConstructorStatic,
                                                             currentNameSpace,
                                                             returnTypeName,
                                                             parameters));
                    }
                    else if (eventDeclarationPattern.IsMatch(data))
                    {
                        var eventHeader = eventDeclarationPattern.Match(data);

                        var eventAccessModifier = eventHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(eventHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var delegateType = eventHeader.Groups["DelegateType"].Value;
                        var eventName = eventHeader.Groups["Name"].Value;

                        events.Add(new EventInfo(eventAccessModifier, delegateType, false, false, eventName, currentNameSpace));
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            var typeInfo = new TypeInfo(acsessModifier,
                                        constructors,
                                        events,
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

        private async Task<TypeInfo> BuildTypeInfoAsync(Node<string> rawTypeInfo)
        {
            if (string.IsNullOrEmpty(currentNameSpace))
            {
                throw new Exception($"{nameof(currentNameSpace)} is null or empty!");
            }

            var data = rawTypeInfo?.Header?.Trim();

            if (string.IsNullOrWhiteSpace(data) || !typeInfoHeaderPattern.IsMatch(data))
            {
                return null;
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
            var events = new List<EventInfo>();

            if (rawTypeInfo.Children != null)
            {
                foreach (var memberInfo in rawTypeInfo.Children)
                {
                    data = memberInfo?.Header?.Trim();

                    if (string.IsNullOrWhiteSpace(data))
                    {
                        continue;
                    }

                    if (!memberInfo.Children?.Any() ?? true && fieldDeclarationPattern.IsMatch(data))
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
                        var parameters = new List<TypeInfo>();

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
                        var parameters = new List<TypeInfo>();

                        var constructorHeader = constructorDeclarationPattern.Match(data);
                        var constructorAcsessModifier = constructorHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(constructorHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var isConstructorAbstract = constructorHeader.Groups["IsAbstract"].Success;
                        var isConstructorStatic = constructorHeader.Groups["IsStatic"].Success;
                        var returnTypeName = constructorHeader.Groups["ReturnTypeName"].Value;

                        constructors.Add(new ConstructorInfo(constructorAcsessModifier,
                                                             isConstructorStatic,
                                                             currentNameSpace,
                                                             returnTypeName,
                                                             parameters));
                    }
                    else if (eventDeclarationPattern.IsMatch(data))
                    {
                        var eventHeader = eventDeclarationPattern.Match(data);

                        var eventAccessModifier = eventHeader.Groups["AccsessModifier"].Success
                         ? Enum.Parse<AcsessModifiers>(eventHeader.Groups["AccsessModifier"].Value, ignoreCase: true)
                         : AcsessModifiers.Private;

                        var delegateType = eventHeader.Groups["DelegateType"].Value;
                        var eventName = eventHeader.Groups["Name"].Value;

                        events.Add(new EventInfo(eventAccessModifier, delegateType, false, false, eventName, currentNameSpace));
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            var typeInfo = new TypeInfo(acsessModifier,
                                        constructors,
                                        events,
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
