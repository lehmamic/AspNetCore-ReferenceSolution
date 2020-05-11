namespace Demo.Backend.CellTowers.Dtos
{
    public class GeoDto
    {
        public string Type { get; set; } = null!;

        public double[] Coordinates { get; set; } = null!;
    }
}