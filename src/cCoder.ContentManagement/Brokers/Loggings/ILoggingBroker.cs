// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;
namespace cCoder.ContentManagement.Brokers.Loggings;

public interface ILoggingBroker
{
    bool IsEnabled(LogLevel logLevel);
    void LogDebug(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}