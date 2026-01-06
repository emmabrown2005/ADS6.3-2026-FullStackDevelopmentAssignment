namespace MapDistanceAPI.Models
{
    public class EdgeDto
    {
        // FromID, ToID, Weight

        public string FromId { get; set; } = string.Empty;
        public string ToId { get; set; } = string.Empty;
        public int Weight { get; set; }
    }
}
