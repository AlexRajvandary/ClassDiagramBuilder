using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder.Models.Parser
{
    public class MemberParser
    {
        private readonly List<string> accsessModifiers = new List<string>() { "private", "public", "protected", "internal", "private protected", "protected internal" };
        private readonly List<string> typekind = new List<string>() { "class", "struct", "enum", "record", "interface", "delegate" };
        private const string abstractStr = "abstract";
        private const string sealedStr = "sealed";
        private const string namespaceStr = "namespace";
        private const string readonlyStr = "readonly";
        private const string constStr = "const";
        private const string usingStr = "using";

        private MemberParser()
        {

        }
        
        public List<TypeInfo> GetFields()
        {
            var fields = new List<TypeInfo>();
            return fields;
        }

        public List<TypeInfo> GetProperties()
        {
            var properties = new List<TypeInfo>();
            return properties;
        }

        public List<MethodInfo> GetMethods()
        {
            var methods = new List<MethodInfo>();
            return methods;
        }

        public List<ConstructorInfo> GetCtors()
        {
            var ctors = new List<ConstructorInfo>();
            return ctors;
        }
    }
}
