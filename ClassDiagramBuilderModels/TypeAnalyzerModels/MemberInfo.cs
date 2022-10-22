namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public abstract class MemberInfo
    {
        public MemberInfo(AcsessModifiers acsessModifier, bool isStatic, string name, string nameSpace)
        {
            AcsessModifier = acsessModifier;
            IsStatic = isStatic;
            Name = name;
            Namespace = nameSpace;
        }

        public AcsessModifiers AcsessModifier { get; private set; }

        public  bool IsStatic { get; private set; }

        public string Name { get; private set; }

        public string Namespace { get; private set; }
    }
}
