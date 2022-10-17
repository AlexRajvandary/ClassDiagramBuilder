namespace ClassDiagramBuilder.Models
{
    public class Node<T>
    {
        private Node<T> parent;

        internal Node() { }

        internal Node(T data)
        {
            Data = data;
        }

        internal Node(Node<T> parent, List<Node<T>> nodes)
        {
            Parent = parent;
            Nodes = nodes;
        }

        internal Node(Node<T> parent, List<Node<T>> nodes, T data)
        {
            Parent = parent;
            Nodes = nodes;
            Data = data;
        }

        public T Data { get; internal set; }

        public string Name { get; internal set; }

        public List<Node<T>> Nodes { get; private set; }

        public Node<T> Parent
        {
            get => parent;
            set
            {
                if (parent == null)
                {
                    parent = value;
                    parent.AddChild(this);
                }
            }
        }

        public void AddChild(Node<T> node)
        {
            Nodes ??= new List<Node<T>>();
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);

                if(node.Parent == null)
                {
                    node.Parent = this;
                }
            }
        }

        public override string ToString() => $"{Data}";

        public void RemoveNode(Node<T> node)
        {
            Nodes?.Remove(node);
        }
    }
}
