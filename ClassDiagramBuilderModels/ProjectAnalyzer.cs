namespace ClassDiagramBuilder.Models
{
    public class ProjectAnalyzer
    {
        public static Node<List<string>> BuildTree(string folder)
        {
            var tree = new Node<List<string>>();
            tree.Name = new DirectoryInfo(folder).Name;
            tree.Data = new List<string>();

            var dirs = Directory.GetDirectories(folder);
            foreach (var dir in dirs)
            {
                var child = BuildTree(dir);
                child.Parent = tree;
                tree.AddNode(child);
            }

            var fileDirs = Directory.GetFiles(folder);

            foreach (var fileDir in fileDirs)
            {
                tree.Data.Add(fileDir);
            }

            return tree;
        }
    }
}