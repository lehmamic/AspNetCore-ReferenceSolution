using System;
using Microsoft.Extensions.Logging;

namespace Demo.Backend.Logging.Dtos
{
    public static class NgxLogMapper
    {
        public static LogLevel ToLogLevel(this NgxLoggerLevelDto dto)
        {
            switch (dto)
            {
                case NgxLoggerLevelDto.TRACE:
                    return LogLevel.Trace;

                case NgxLoggerLevelDto.DEBUG:
                    return LogLevel.Debug;

                case NgxLoggerLevelDto.INFO:
                    return LogLevel.Information;

                case NgxLoggerLevelDto.LOG:
                    return LogLevel.Information;

                case NgxLoggerLevelDto.WARN:
                    return LogLevel.Warning;

                case NgxLoggerLevelDto.ERROR:
                    return LogLevel.Error;

                case NgxLoggerLevelDto.FATAL:
                    return LogLevel.Error;

                case NgxLoggerLevelDto.OFF:
                    return LogLevel.None;

                default:
                    throw new NotImplementedException($"No mapping for log level {dto} implemented");
            }
        }
    }
}