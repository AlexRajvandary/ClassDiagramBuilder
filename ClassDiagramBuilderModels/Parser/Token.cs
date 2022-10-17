namespace ClassDiagramBuilder.Models.Parser
{
    public class Token
    {
        public Token()
        {

        }

        public Token(string value)
        {
            Value = value;
        }

        public Token(string name, string value)
        {
            Header = name;
            Value = value;
        }

        public string Header { get; set; }

        public string Value { get; set; }
    }
}
