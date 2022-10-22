namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class TypeInfo : MemberInfo
    {
        public TypeInfo(AcsessModifiers acsessModifier,
                        bool isStatic,
                        TypeKind typeKind,
                        string name,
                        string nameSpace) : this(acsessModifier, null, null, isStatic, typeKind, null, name, nameSpace, null) { }

        public TypeInfo(AcsessModifiers acsessModifier,
                        List<ConstructorInfo> constructors,
                        List<TypeInfo> fields,
                        bool isStatic,
                        TypeKind typeKind,
                        List<MethodInfo> methods,
                        string name,
                        string nameSpace,
                        List<TypeInfo> properties) : base(acsessModifier, isStatic, name, nameSpace)
        {
            Constructors = constructors;
            Fields = fields;
            Kind = typeKind;
            Methods = methods;
            Properties = properties;
        }

        public List<ConstructorInfo>? Constructors { get; set; }

        public List<TypeInfo>? Fields { get; set; }

        public TypeKind Kind { get; set; }

        public List<MethodInfo>? Methods { get; set; }

        public List<TypeInfo>? Properties { get; set; }

        public override string ToString() => $"{Namespace}.{Name}";
    }
}
