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
                         string typeName) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            IsConstant = isConstant;
            IsReadOnly = isReadOnly;
            TypeName = typeName;
        }

        public bool IsConstant { get; private set; }

        public bool IsReadOnly { get; private set; }

        public string TypeName { get; private set; }
    }
}