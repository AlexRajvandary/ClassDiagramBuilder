namespace ClassDiagramBuilder
{
    public class GraphEdge<T>
    {
        public GraphVertex<T> ConnectedVertex { get; }

        public int EdgeWeight { get; }

        public GraphEdge(GraphVertex<T> connectedVertex, int weight)
        {
            ConnectedVertex = connectedVertex;
            EdgeWeight = weight;
        }
    }
}