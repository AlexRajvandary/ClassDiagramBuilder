namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class ConstructorInfo : MemberInfo
    {
        public ConstructorInfo(AcsessModifiers acsessModifier,
                               bool isStatic,
                               string nameSpace,
                               string returnTypeName,
                               List<TypeInfo> parameters) : base(acsessModifier, false, isStatic, null, nameSpace)
        {
            Parameters = parameters;
            ReturnTypeName = returnTypeName;
        }

        public List<TypeInfo> Parameters { get; private set; }

        public string ReturnTypeName { get; private set; }

        public override string ToString()
        {
            return $"{ReturnTypeName}()";
        }
    }
}
