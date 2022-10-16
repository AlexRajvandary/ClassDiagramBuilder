﻿namespace ClassDiagramBuilder.Models
{
    public class ProjectAnalyzer
    {
        public List<string> FileExtensionsToAnalyze { get; set; }

        public List<string> FileExtensionsToIgnore { get; set; }

        public List<string> FoldersToIgnore { get; set; }

        public Node<List<string>> BuildTree(string folder)
        {
            var tree = new Node<List<string>>();
            tree.Name = new DirectoryInfo(folder).Name;
            tree.Data = new List<string>();

            var dirs = FoldersToIgnore == null
                ? Directory.GetDirectories(folder)
                : Directory.GetDirectories(folder).Where(folderPath => !FoldersToIgnore.Contains(new DirectoryInfo(folderPath).Name));

            foreach (var dir in dirs)
            {
                var child = BuildTree(dir);
                child.Parent = tree;
                tree.AddNode(child);
            }

            var fileDirs = FileExtensionsToAnalyze != null
                ? Directory.GetFiles(folder).ToList().Where(filePath => FileExtensionsToAnalyze.Contains(Path.GetExtension(filePath)))
                : FileExtensionsToIgnore != null
                    ? Directory.GetFiles(folder).ToList().Where(filePath => !FileExtensionsToAnalyze.Contains(Path.GetExtension(filePath)))
                    : Directory.GetFiles(folder);

            foreach (var fileDir in fileDirs)
            {
                tree.Data.Add(fileDir);
            }

            return tree;
        }
    }
}