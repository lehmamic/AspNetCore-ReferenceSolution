namespace Demo.Backend.CellTowers.Dtos
{
    public class ParametersDto
    {
        public string Dataset { get; set; } = null!;

        public string Timezone { get; set; } = null!;

        public int Rows { get; set; }

        public string Format { get; set; } = null!;

        public string[] Facet { get; set; } = null!;
    }
}