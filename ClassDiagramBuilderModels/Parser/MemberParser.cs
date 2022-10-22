using ClassDiagramBuilder.Models.TypeAnalyzerModels;
using System.Text.RegularExpressions;

namespace ClassDiagramBuilder.Models.Parser
{
    public class MemberParser
    {
        private readonly Regex classDeclarationPattern = new Regex(@"");
        private readonly Regex fieldDeclarationPattern = new Regex(@"");
        private readonly Regex propertyDeclarationPattern = new Regex(@"");
        private readonly Regex methodDeclarationPattern = new Regex(@"");
        private readonly Regex constructorDeclarationPattern = new Regex(@"");
        private readonly List<string> accsessModifiers = new List<string>() { "private", "public", "protected", "internal", "private protected", "protected internal" };
        private readonly List<string> typekind = new List<string>() { "class", "struct", "enum", "record", "interface", "delegate" };
        private const string abstractStr = "abstract";
        private const string sealedStr = "sealed";
        private const string namespaceStr = "namespace";
        private const string readonlyStr = "readonly";
        private const string constStr = "const";
        private const string usingStr = "using";

        public MemberParser()
        {

        }

        public MemberInfo Parse(string data)
        {
            throw new NotImplementedException();
        }
    }
}
