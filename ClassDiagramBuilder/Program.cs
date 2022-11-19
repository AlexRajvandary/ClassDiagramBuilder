using ClassDiagramBuilder.Models;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace ClassDiagramBuilder
{
    internal class Program
    {
        private static Node<List<string>> filesTree;
        private static ProjectAnalyzer projectAnalyzer;
        private static readonly string outputFilePath = @"output.txt";

        static void Main(string[] args)
        {
            File.Create(outputFilePath);

            projectAnalyzer = new ProjectAnalyzer();
            projectAnalyzer.FileExtensionsToAnalyze = new List<string>() { @".cs" };
            projectAnalyzer.FoldersToIgnore = new List<string>() { @".git", @".vs", @"bin", @"obj" };

            filesTree = projectAnalyzer.BuildTree(@"C:\Users\Alex Raj\source\repos\ClassDiagramBuilder\ClassDiagramBuilder");

            PrintTreesData(filesTree, Console.WriteLine);

            Console.ReadKey();
        }

        static string GetRoot(string path, int level)
        {
            if (level == 0) return path;
            return GetRoot(Directory.GetParent(path).ToString(), level - 1);
        }

        static void PrintTree<T>(Node<T> node, Action<string> printer)
        {
            printer?.Invoke(node?.ToString());

            if (node?.Children?.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    PrintTree(child, printer);
                }
            }
        }

        static void PrintTreesData(Node<List<string>> node, Action<string> printer)
        {
            foreach (var file in node.Data)
            {
                printer?.Invoke(file);
            }

            if (node?.Children?.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    PrintTreesData(child, printer);
                }
            }
        }
    }
}