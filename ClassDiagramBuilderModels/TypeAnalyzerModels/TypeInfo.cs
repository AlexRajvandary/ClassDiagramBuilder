namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class TypeInfo : MemberInfo
    {
        private TypeInfo(bool isAbstract,
                         bool isStatic,
                         TypeKind typeKind,
                         string name,
                         string nameSpace) : base(AcsessModifiers.Private, isAbstract, isStatic, name, nameSpace)
        {
            Kind = typeKind;
        }
        public TypeInfo(AcsessModifiers acsessModifier,
                        bool isAbstract,
                        bool isStatic,
                        TypeKind typeKind,
                        string name,
                        string nameSpace) : this(acsessModifier, null, null, isAbstract, isStatic, null, name, nameSpace, null, typeKind) { }

        public TypeInfo(AcsessModifiers acsessModifier,
                        List<ConstructorInfo> constructors,
                        List<FieldInfo> fields,
                        bool isAbstract,
                        bool isStatic,
                        List<MethodInfo> methods,
                        string name,
                        string nameSpace,
                        List<PropertyInfo> properties,
                        TypeKind typeKind) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            Constructors = constructors;
            Fields = fields;
            Kind = typeKind;
            Methods = methods;
            Properties = properties;
        }

        public TypeInfo BaseClass { get; private set; }

        public List<TypeInfo> BaseInterfaces { get; private set; } = new List<TypeInfo>();

        public List<ConstructorInfo> Constructors { get; private set; } = new List<ConstructorInfo>();

        public bool Created { get; private set; }

        public List<FieldInfo> Fields { get; private set; } = new List<FieldInfo>();

        public TypeKind Kind { get; private set; }

        public List<MethodInfo> Methods { get; private set; } = new List<MethodInfo>();

        public List<PropertyInfo> Properties { get; private set; } = new List<PropertyInfo>();

        public override string ToString() => $"{Namespace}.{Name}";

        public static TypeInfo GetUnfilledTypeInfo(string name) => new TypeInfo(false, false, TypeKind.Undefined, name, nameSpace: null);
    }
}
