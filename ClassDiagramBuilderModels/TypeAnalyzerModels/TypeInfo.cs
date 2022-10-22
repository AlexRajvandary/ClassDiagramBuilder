namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class TypeInfo : MemberInfo
    {
        public TypeInfo(AcsessModifiers acsessModifier,
                        bool isStatic,
                        TypeKind typeKind,
                        string name,
                        string nameSpace) : this(acsessModifier, null, null, isStatic, null, name, nameSpace, null, typeKind) { }

        public TypeInfo(AcsessModifiers acsessModifier,
                        List<ConstructorInfo> constructors,
                        List<TypeInfo> fields,
                        bool isStatic,
                        List<MethodInfo> methods,
                        string name,
                        string nameSpace,
                        List<TypeInfo> properties,
                        TypeKind typeKind) : base(acsessModifier, isStatic, name, nameSpace)
        {
            Constructors = constructors;
            Fields = fields;
            Kind = typeKind;
            Methods = methods;
            Properties = properties;
        }

        public List<ConstructorInfo>? Constructors { get; private set; }

        public bool Created { get; private set; }

        public List<TypeInfo>? Fields { get; private set; }

        public TypeKind Kind { get; private set; }

        public List<MethodInfo>? Methods { get; private set; }

        public List<TypeInfo>? Properties { get; private set; }

        public override string ToString() => $"{Namespace}.{Name}";
    }
}
