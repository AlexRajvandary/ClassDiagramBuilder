namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class MethodInfo
    {
        public AcsessModifiers AcessModifiers { get; set; }

        public string Name { get; set; }

        public List<TypeInfo> Parameters { get; set; }

        public TypeInfo ReturnType { get; set; }
    }
}
