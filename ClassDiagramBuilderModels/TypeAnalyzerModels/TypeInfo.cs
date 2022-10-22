namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class TypeInfo
    {
        public TypeInfo(AcsessModifiers acsessModifier,
                        TypeKind typeKind,
                        string name,
                        string nameSpace) : this(acsessModifier, null, typeKind, null, name, nameSpace, null) { }

        public TypeInfo(AcsessModifiers acsessModifier,
                        List<TypeInfo> fields,
                        TypeKind typeKind,
                        List<MethodInfo> methods,
                        string name,
                        string nameSpace,
                        List<TypeInfo> properties)
        {
            AcsessModifier = acsessModifier;
            Fields = fields;
            Kind = typeKind;
            Methods = methods;
            Name = name;
            Namespace = nameSpace;
            Properties = properties;
        }

        public AcsessModifiers AcsessModifier { get; set; }

        public List<TypeInfo> Fields { get; set; }

        public TypeKind Kind { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public List<TypeInfo> Properties { get; set; }

        public override string ToString() => $"{Namespace}.{Name}";
    }
}
