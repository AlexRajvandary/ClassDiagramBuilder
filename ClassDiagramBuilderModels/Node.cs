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

        internal Node(Node<T> parent, List<Node<T>> children)
        {
            Parent = parent;
            Children = children;
        }

        internal Node(Node<T> parent, List<Node<T>> children, T data)
        {
            Parent = parent;
            Children = children;
            Data = data;
        }

        public List<Node<T>> Children { get; private set; }

        public T Data { get; internal set; }

        public string Name { get; internal set; }

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
            Children ??= new List<Node<T>>();
            if (!Children.Contains(node))
            {
                Children.Add(node);

                if(node.Parent == null)
                {
                    node.Parent = this;
                }
            }
        }

        public override string ToString() => $"{Data}";

        public void RemoveNode(Node<T> node)
        {
            Children?.Remove(node);
        }
    }
}
