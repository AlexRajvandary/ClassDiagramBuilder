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

        public string Header { get; internal set; }

        public bool HeaderReadOnly => Children != null && Children.Count != 0;

        public int Level => Parent?.Level + 1 ?? 1;

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

        public void AddChild(Node<T> child)
        {
            Children ??= new List<Node<T>>();
            if (!Children.Contains(child))
            {
                Children.Add(child);

                if(child.Parent == null)
                {
                    child.Parent = this;
                }
            }
        }

        public override string ToString() => $"{Header}";

        public void RemoveChild(Node<T> node)
        {
            node.Parent = null;
            Children?.Remove(node);
        }
    }
}
