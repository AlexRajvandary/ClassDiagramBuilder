namespace ClassDiagramBuilder
{
    public class Graph<T>
    {
        public List<GraphVertex<T>> Vertices { get; }

        public Graph()
        {
            Vertices = new List<GraphVertex<T>>();
        }

        public void AddVertex(string vertexName)
        {
            Vertices.Add(new GraphVertex<T>(vertexName));
        }

        public GraphVertex<T> FindVertex(string vertexName)
        {
            foreach (var v in Vertices)
            {
                if (v.Name.Equals(vertexName))
                {
                    return v;
                }
            }

            return null;
        }
    }
}