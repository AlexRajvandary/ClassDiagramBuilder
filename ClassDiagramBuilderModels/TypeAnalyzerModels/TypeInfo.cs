namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class TypeInfo
    {
        public AcessModifiers AcessModifiers { get; set; }

        public TypeKind Kind { get; set; }

        public string Name { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public List<TypeInfo> Properties { get; set; }
    }
}
