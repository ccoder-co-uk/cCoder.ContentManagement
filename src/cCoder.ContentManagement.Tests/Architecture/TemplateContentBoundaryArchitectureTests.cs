// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class TemplateContentBoundaryArchitectureTests
{
    [Fact]
    public void TemplatePath_WhenComposed_ShouldRespectEachLayerBoundary()
    {
        // Given
        Type[] expectedTemplateContentOrchestrationDependencies =
        [
            typeof(ITemplateContentProcessingService),
            typeof(IHtmlToPdfProcessingService)
        ];

        Type[] expectedTemplateManagerAggregationDependencies =
        [
            typeof(ITemplateOrchestrationService),
            typeof(ITemplateContentOrchestrationService)
        ];

        // When
        Type[] templateServiceDependencies = ConstructorDependencies(type: typeof(TemplateService));
        Type[] templateContentServiceDependencies = ConstructorDependencies(type: typeof(TemplateContentService));
        Type[] htmlToPdfServiceDependencies = ConstructorDependencies(type: typeof(HtmlToPdfService));
        Type[] templateProcessingDependencies = ConstructorDependencies(type: typeof(TemplateProcessingService));
        Type[] templateContentProcessingDependencies = ConstructorDependencies(type: typeof(TemplateContentProcessingService));
        Type[] htmlToPdfProcessingDependencies = ConstructorDependencies(type: typeof(HtmlToPdfProcessingService));
        Type[] templateContentOrchestrationDependencies = ConstructorDependencies(type: typeof(TemplateContentOrchestrationService));
        Type[] templateManagerAggregationDependencies = ConstructorDependencies(type: typeof(TemplateManagerAggregationService));
        Type[] templateManagerDependencies = ConstructorDependencies(type: typeof(TemplateManager));

        // Then
        templateServiceDependencies.SequenceEqual(second: [typeof(ITemplateBroker)])
            .Should()
            .BeTrue();

        templateContentServiceDependencies.SequenceEqual(second: [typeof(ITemplateStreamBroker)])
            .Should()
            .BeTrue();

        htmlToPdfServiceDependencies.SequenceEqual(second: [typeof(IHtmlToPdfBroker)])
            .Should()
            .BeTrue();

        templateProcessingDependencies.SequenceEqual(second: [typeof(ITemplateService)])
            .Should()
            .BeTrue();

        templateContentProcessingDependencies.SequenceEqual(second: [typeof(ITemplateContentService)])
            .Should()
            .BeTrue();

        htmlToPdfProcessingDependencies.SequenceEqual(second: [typeof(IHtmlToPdfService)])
            .Should()
            .BeTrue();

        templateContentOrchestrationDependencies.SequenceEqual(
                second: expectedTemplateContentOrchestrationDependencies)
            .Should()
            .BeTrue();

        templateManagerAggregationDependencies.SequenceEqual(
                second: expectedTemplateManagerAggregationDependencies)
            .Should()
            .BeTrue();

        templateManagerDependencies.SequenceEqual(
                second: [typeof(ITemplateManagerAggregationService)])
            .Should()
            .BeTrue();
    }

    [Fact]
    public void TemplateBrokers_WhenEvaluated_ShouldNotMasqueradeAsUtilityBrokers()
    {
        // Given
        Type utilityBrokerType = typeof(IUtilityBroker);

        // When
        bool isTemplateStreamUtility = utilityBrokerType.IsAssignableFrom(c: typeof(TemplateStreamBroker));
        bool isHtmlToPdfUtility = utilityBrokerType.IsAssignableFrom(c: typeof(HtmlToPdfBroker));

        // Then
        isTemplateStreamUtility
            .Should()
            .BeFalse();

        isHtmlToPdfUtility
            .Should()
            .BeFalse();

        typeof(ITemplateStreamBroker).IsPublic
            .Should()
            .BeFalse();

        typeof(IHtmlToPdfBroker).IsPublic
            .Should()
            .BeFalse();
    }

    private static Type[] ConstructorDependencies(Type type) =>
        type.GetConstructors(bindingAttr:
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();
}