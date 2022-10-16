namespace ClassDiagramBuilder.Models
{
    public class Node<T>
    {
        internal Node() { }

        internal Node(Node<T> parent, List<Node<T?>> nodes)
        {
            Parent = parent;
            Nodes = nodes;
        }

        internal Node(Node<T> parent, List<Node<T?>> nodes, T data)
        {
            Parent = parent;
            Nodes = nodes;
            Data = data;
        }

        public T Data { get; internal set; }

        public string Name { get; internal set; }

        public List<Node<T>> Nodes { get; private set; }

        public Node<T> Parent { get; internal set; }

        public void AddNode(Node<T> node)
        {
            Nodes ??= new List<Node<T>>();
            Nodes.Add(node);
        }

        public void RemoveNode(Node<T> node)
        {
            Nodes?.Remove(node);
        }
    }
}
