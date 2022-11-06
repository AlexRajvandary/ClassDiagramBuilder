using ClassDiagramBuilder.Models;

namespace ClassDiagramBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var projectAnalyzer = new ProjectAnalyzer();
            projectAnalyzer.FileExtensionsToAnalyze = new List<string>() { @".cs" };
            projectAnalyzer.FoldersToIgnore = new List<string>() { @".git", @".vs", @"bin", @"obj" };

            var filesTree = projectAnalyzer.BuildTree(@"C:\Users\Alex Raj\source\repos\ClassDiagramBuilder");

            var syntaxTree = projectAnalyzer.AnalyzeFile(filesTree.Children[0].Data[0]);
            PrintTree(syntaxTree);
            Console.ReadKey();
        }

        static string GetRoot(string path, int level)
        {
            if (level == 0) return path;
            return GetRoot(Directory.GetParent(path).ToString(), level - 1);
        }

        static void PrintTree<T>(Node<T> node)
        {
            Console.WriteLine(new string('\t', node.Level) + node.Header);
            if (node.Children?.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    PrintTree(child);
                }
            }
        }
    }
}