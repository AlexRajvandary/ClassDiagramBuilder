using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class TypeParser
    {
        public void Parse(string path)
        {
            string data;
            using (var sr = new StreamReader(path))
            {
               data = sr.ReadToEnd();
            }

            data = data.Replace(Environment.NewLine, string.Empty);
            var assembliesDeclarationPart = data.Substring(0, data.IndexOf('{'));
            var classDeclarationPart = data.Substring(data.IndexOf('{'));

            var typeDeclare = classDeclarationPart.Substring(0, classDeclarationPart.IndexOf('{')).Split(' ');
            var accsessModifier = typeDeclare[0];
            var TypeKind = typeDeclare[1];
            var TypeName = typeDeclare[2];

            var typeDefinition = classDeclarationPart.Substring(classDeclarationPart.IndexOf('{'), classDeclarationPart.IndexOf('}'));
        }
    }
}
