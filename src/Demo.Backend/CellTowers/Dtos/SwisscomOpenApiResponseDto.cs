using System.Text.Json.Serialization;

namespace Demo.Backend.CellTowers.Dtos
{
    public class SwisscomOpenApiResponseDto
    {
        public int Nhits { get; set; }

        public ParametersDto Parameters { get; set; } = null!;

        public RecordDto[] Records { get; set; } = null!;

        [JsonPropertyName("record_timestamp")]
        public SwisscomOpenApis.Dtos.FacetGroupDto[] FacetGroups { get; set; } = null!;
    }
}