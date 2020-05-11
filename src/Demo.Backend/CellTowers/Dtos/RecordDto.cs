using System;
using System.Text.Json.Serialization;

namespace Demo.Backend.CellTowers.Dtos
{
    public class RecordDto
    {
        public string DatasetId { get; set; } = null!;

        public string RecordId { get; set; } = null!;

        public SwisscomOpenApis.Dtos.FieldsDto Fields { get; set; } = null!;

        public SwisscomOpenApis.Dtos.GeoDto Geometry { get; set; } = null!;

        [JsonPropertyName("record_timestamp")]
        public DateTimeOffset RecordTimestamp { get; set; }
    }
}