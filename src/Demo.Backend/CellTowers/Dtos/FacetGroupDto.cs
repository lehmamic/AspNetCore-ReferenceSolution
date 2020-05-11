namespace Demo.Backend.CellTowers.Dtos
{
    public class FacetGroupDto
    {
        public SwisscomOpenApis.Dtos.FacetDto[] Facets { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}