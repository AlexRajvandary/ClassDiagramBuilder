namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class MethodInfo : MemberInfo
    {
        public MethodInfo(AcsessModifiers acsessModifier,
                         bool isAbstract,
                         bool isStatic,
                         string name,
                         string nameSpace,
                         List<TypeInfo> parameters,
                         string returnTypeName) : this(acsessModifier, isAbstract, isStatic, name, nameSpace, parameters, TypeInfo.GetUnfilledTypeInfo(returnTypeName))
        {
           
        }

        public MethodInfo(AcsessModifiers acsessModifier,
                          bool isAbstract,
                          bool isStatic,
                          string name,
                          string nameSpace,
                          List<TypeInfo> parameters,
                          TypeInfo returnType) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public List<TypeInfo> Parameters { get; private set; }

        public TypeInfo ReturnType { get; private set; }

        public override string ToString() => $"{ReturnType.Name} {Name} ()";
    }
}
