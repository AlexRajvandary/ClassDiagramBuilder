using ClassDiagramBuilder.Models;
using System.Reflection.Emit;

namespace ClassDiagramBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int y = 0;
            var projectAnalyzer = new ProjectAnalyzer();
            projectAnalyzer.FileExtensionsToAnalyze = new List<string>(){@".cs"};
            projectAnalyzer.FoldersToIgnore = new List<string>() { @".git", @".vs" };

            var tree = projectAnalyzer.BuildTree(GetRoot(Environment.CurrentDirectory, 4));
            Console.ReadKey();
        }

        static string GetRoot(string path, int level)
        {
            if (level == 0) return path;
            return GetRoot(Directory.GetParent(path).ToString(), level - 1);
        }
    }
}