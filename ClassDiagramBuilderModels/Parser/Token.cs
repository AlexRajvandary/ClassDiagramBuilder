namespace ClassDiagramBuilder.Models.Parser
{
    public class Token
    {
        private static List<string> keyWords = new List<string>() { "namespace", "class", "struct", "enum"};

        public Token()
        {

        }

        public Token(string value)
        {
            Value = value;
        }

        public Token(string header, string value)
        {
            Header = header;
            Value = value;
        }

        public string Header { get; set; }

        public string Value { get; set; }

        public TokenLevel TokenLevel { get; set; }

        public TokenType TokenType { get; set; }

        //public static bool TryParse(string str, out Token? token)
        //{
        //    if(str.Length == 0)
        //    {
        //        token = null;
        //        return false;
        //    }

        //    var data = str.Split(";");
            
        //    foreach(var keyWord in keyWords)
        //    {
        //        if (data[^1].Contains(keyWord))
        //        {

        //        }
        //    }
        //}
    }
}
