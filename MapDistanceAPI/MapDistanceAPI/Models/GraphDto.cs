namespace MapDistanceAPI.Models
{
    public class GraphDto
    {
        //Nodes List<NodeDto>, Edges List<EdgeDto>
        public List<NodeDto> Nodes { get; set; } = new();
        public List<EdgeDto> Edges { get; set; } = new();

    }
}
