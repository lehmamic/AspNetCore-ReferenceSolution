namespace Demo.Backend.CellTowers.Dtos
{
    public class FieldsDto
    {
        public GeoDto Geoshape { get; set; } = null!;

        public string Powercode { get; set; } = null!;

        public double[] Geopoint { get; set; } = null!;

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int Id { get; set; }
    }
}