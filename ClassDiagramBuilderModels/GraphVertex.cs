namespace ClassDiagramBuilder
{
    public class GraphVertex<T>
    {
        public string Name { get; }

        public List<GraphEdge<T>> Edges { get; }

        public GraphVertex(string vertexName)
        {
            Name = vertexName;
            Edges = new List<GraphEdge<T>>();
        }

        public void AddEdge(GraphEdge<T> newEdge)
        {
            Edges.Add(newEdge);
        }

        public override string ToString() => Name;
    }
}