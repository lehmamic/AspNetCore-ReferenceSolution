using System.Text.Json.Serialization;

namespace Demo.Backend.SwisscomOpenApis.Dtos
{
    public class SwisscomOpenApiResponseDto
    {
        public int Nhits { get; set; }

        public ParametersDto Parameters { get; set; } = null!;

        public RecordDto[] Records { get; set; } = null!;

        [JsonPropertyName("record_timestamp")]
        public FacetGroupDto[] FacetGroups { get; set; } = null!;
    }
}