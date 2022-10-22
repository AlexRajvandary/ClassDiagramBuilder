namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class MethodInfo : MemberInfo
    {
        public MethodInfo(AcsessModifiers acsessModifier,
                          bool isStatic,
                          string name,
                          string nameSpace,
                          List<TypeInfo> parameters,
                          TypeInfo returnType) : base(acsessModifier, isStatic, name, nameSpace)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public List<TypeInfo> Parameters { get; private set; }

        public TypeInfo ReturnType { get; private set; }
    }
}
