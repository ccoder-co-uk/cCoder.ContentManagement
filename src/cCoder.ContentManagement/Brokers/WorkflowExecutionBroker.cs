// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Brokers;

internal sealed class WorkflowExecutionBroker(
    WorkflowExecutionDependency workflowExecutionDependency)
        : IWorkflowExecutionBroker
{
    public string Execute(string baseAddress, string content) =>
        workflowExecutionDependency.Execute(
            baseAddress: baseAddress,
            content: content);
}