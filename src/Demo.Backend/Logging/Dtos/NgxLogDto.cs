using System;

namespace Demo.Backend.Logging.Dtos
{
    public class NgxLogDto
    {
        public NgxLoggerLevelDto Level { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string FileName { get; set; } = null!;

        public string LineNumber { get; set; } = null!;

        public string Message { get; set; } = null!;

        public object[] Additional { get; set; } = new object[0];
    }
}