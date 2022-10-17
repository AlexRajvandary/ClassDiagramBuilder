using ClassDiagramBuilder.Models;
using System.ComponentModel.Design;
using System.Reflection.Emit;

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
            
            var syntaxTree = projectAnalyzer.AnalyzeFile(filesTree.Nodes[0].Data[0]);
            PrintTree(syntaxTree, 0);
            Console.ReadKey();
        }

        static string GetRoot(string path, int level)
        {
            if (level == 0) return path;
            return GetRoot(Directory.GetParent(path).ToString(), level - 1);
        }

        static void PrintTree<T>(Node<T> tree, int level)
        {
            Console.WriteLine(tree);
            if (tree.Nodes?.Count > 0)
            {
                foreach (var child in tree.Nodes)
                {
                    PrintTree(child, level);
                }
            }
        }
    }
}