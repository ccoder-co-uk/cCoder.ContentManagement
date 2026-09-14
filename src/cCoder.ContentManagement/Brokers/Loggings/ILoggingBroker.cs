// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.ContentManagement.Brokers.Loggings;

public interface ILoggingBroker : IUtilityBroker
{
    bool IsEnabled(LogLevel logLevel);
    void LogDebug(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}