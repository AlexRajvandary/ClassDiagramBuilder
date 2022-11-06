namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class ConstructorInfo : MemberInfo
    {
        public ConstructorInfo(AcsessModifiers acsessModifier,
                               bool isStatic,
                               string nameSpace,
                               List<TypeInfo> parameters) : base(acsessModifier, false, isStatic, null, nameSpace)
        {
            Parameters = parameters;
        }

        public List<TypeInfo> Parameters { get; private set; }
    }
}
