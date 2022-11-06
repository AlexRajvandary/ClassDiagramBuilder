namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class FieldInfo : MemberInfo
    {
        public FieldInfo(AcsessModifiers acsessModifier,
                         bool isAbstract,
                         bool isConstant,
                         bool isStatic,
                         bool isReadOnly,
                         string name,
                         string nameSpace,
                         TypeInfo type) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            IsConstant = isConstant;
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsConstant { get; private set; }

        public bool IsReadOnly { get; private set; }

        public TypeInfo Type { get; private set; }
    }
}