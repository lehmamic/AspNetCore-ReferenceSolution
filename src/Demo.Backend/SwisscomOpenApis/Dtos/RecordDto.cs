using System;
using System.Text.Json.Serialization;

namespace Demo.Backend.SwisscomOpenApis.Dtos
{
    public class RecordDto
    {
        public string DatasetId { get; set; } = null!;

        public string RecordId { get; set; } = null!;

        public FieldsDto Fields { get; set; } = null!;

        public GeoDto Geometry { get; set; } = null!;

        [JsonPropertyName("record_timestamp")]
        public DateTimeOffset RecordTimestamp { get; set; }
    }
}