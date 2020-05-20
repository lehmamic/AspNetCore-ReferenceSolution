// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace Demo.Backend.Logging.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NgxLoggerLevelDto
    {
        TRACE = 0,
        DEBUG = 1,
        INFO = 2,
        LOG = 3,
        WARN = 4,
        ERROR = 5,
        FATAL = 6,
        OFF = 7,
    }
}