using ClassDiagramBuilder.Models;
using System.Reflection.Emit;

namespace ClassDiagramBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var projectAnalyzer = new ProjectAnalyzer();
            projectAnalyzer.FileExtensionsToAnalyze = new List<string>(){@".cs"};
            projectAnalyzer.FoldersToIgnore = new List<string>() { @".git", @".vs", @"bin", @"obj" };

            var tree = projectAnalyzer.BuildTree(GetRoot(Environment.CurrentDirectory, 4));
            projectAnalyzer.AnalyzeFile(tree.Nodes[0].Data[0]);
            Console.ReadKey();
        }

        static string GetRoot(string path, int level)
        {
            if (level == 0) return path;
            return GetRoot(Directory.GetParent(path).ToString(), level - 1);
        }
    }
}