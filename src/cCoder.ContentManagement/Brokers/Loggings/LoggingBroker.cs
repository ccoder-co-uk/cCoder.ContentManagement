// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.Logging;

namespace cCoder.ContentManagement.Brokers.Loggings;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger)
    : ILoggingBroker, IUtilityBroker
{
    public bool IsEnabled(LogLevel logLevel) =>
        logger.IsEnabled(logLevel: logLevel);

    public void LogDebug(string message, params object[] args) =>
        logger.LogDebug(message: message, args: args);

    public void LogError(Exception exception, string message, params object[] args) =>
        logger.LogError(exception: exception, message: message, args: args);
}
