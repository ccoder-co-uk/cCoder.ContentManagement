// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace cCoder.ContentManagement.Brokers.Loggings;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger) : ILoggingBroker
{
    public bool IsEnabled(LogLevel logLevel) =>
        logger.IsEnabled(logLevel: logLevel);

    public void LogDebug(string message, params object[] args) =>
        logger.LogDebug(message: message, args: args);

    public void LogError(Exception exception, string message, params object[] args) =>
        logger.LogError(exception: exception, message: message, args: args);
}