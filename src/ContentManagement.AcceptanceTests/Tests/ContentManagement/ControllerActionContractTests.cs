// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT003, STXFORMAT005, STXTEST005

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cCoder.ContentManagement.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class ControllerActionContractTests
{
    private static readonly Assembly ProductAssembly = Assembly.LoadFile(Path.Combine(AppContext.BaseDirectory, "cCoder.ContentManagement.dll"));

    public static IEnumerable<object[]> Actions => ProductAssembly.GetTypes()
        .Where(type => type.Namespace == "cCoder.ContentManagement.Exposures.Controllers" && type.Name.EndsWith("Controller"))
        .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        .Where(IsAction)
        .Select(method => new object[] { method })
        .ToArray();

    public static IEnumerable<object[]> HappyGapActions => Actions
        .Where(data => IsHappyGap(action: (MethodInfo)data[0]));

    public static IEnumerable<object[]> ModelStateBranches => new[]
    {
        "AppController.Post", "AppController.Put", "AppCultureController.Post", "AppCultureController.DeleteAll",
        "CommonObjectController.Post", "CommonObjectController.Put", "ComponentController.Post", "ComponentController.Put",
        "ContentController.Post", "ContentController.Put", "CultureController.Post", "CultureController.Put",
        "LayoutController.Post", "LayoutController.Put", "PackageController.PostImportAsync", "PageController.Post",
        "PageController.Put", "PageInfoController.Post", "PageInfoController.Put", "PageRenderCacheController.Post",
        "PageRenderCacheController.Put", "PageRoleController.Post", "PageRoleController.DeleteAll", "ResourceController.Post",
        "ResourceController.Put", "ScriptController.Post", "ScriptController.Put", "SubmissionController.Post",
        "SubmissionController.Put", "TemplateController.Post", "TemplateController.Put"
    }.Select(actionName => new object[] { FindAction(actionName: actionName) });

    public static IEnumerable<object[]> NotFoundBranches => new[]
    {
        "AppController.GetIsAdmin", "AppController.Get", "AppUserController.GetUsers", "CommonObjectController.GetLatest",
        "CommonObjectController.Get", "ComponentController.Get", "ContentController.Get", "CultureController.Get",
        "LayoutController.Get", "PageController.GetRootFor", "PageController.GetMenu", "PageController.Get",
        "PageInfoController.Get", "PageRenderCacheController.Get/1", "RenderController.GetComponent", "ResourceController.Get",
        "ScriptController.Get", "SubmissionController.Get", "TemplateController.Get"
    }.Select(actionName => new object[] { FindAction(actionName: actionName) });

    [Theory]
    [MemberData(nameof(HappyGapActions))]
    public async Task Action_WhenDependencyCompletes_ShouldReturnSuccessAndLogInformation(MethodInfo action)
    {
        // Given / When
        Outcome outcome = await Invoke(action: action, failure: null);

        // Then
        int? statusCode = (outcome.Result as IStatusCodeActionResult)?.StatusCode;
        Assert.True(condition: statusCode is null or >= 200 and < 400, userMessage: $"{action.DeclaringType!.Name}.{action.Name} returned {statusCode}.");
        Assert.Equal(expected: 1, actual: outcome.Logging.InformationCount);
        Assert.Equal(expected: 0, actual: outcome.Logging.WarningCount);
        Assert.Equal(expected: 0, actual: outcome.Logging.ErrorCount);
    }

    [Theory]
    [MemberData(nameof(ModelStateBranches))]
    public async Task Action_WhenModelStateIsInvalid_ShouldReturn400AndWarn(MethodInfo action)
    {
        // Given / When
        Outcome outcome = await Invoke(action: action, failure: null, invalidModelState: true);

        // Then
        AssertDirectClientError(outcome: outcome, expectedStatus: 400);
    }

    [Theory]
    [MemberData(nameof(NotFoundBranches))]
    public async Task Action_WhenResultIsMissing_ShouldReturn404AndWarn(MethodInfo action)
    {
        // Given / When
        bool invalidRoute = action.DeclaringType!.Name is "AppController" && action.Name == "GetIsAdmin"
            || action.DeclaringType.Name == "AppUserController";

        Outcome outcome = await Invoke(action: action, failure: null, returnNull: !invalidRoute, nullArguments: invalidRoute);

        // Then
        AssertDirectClientError(outcome: outcome, expectedStatus: 404);
    }

    [Fact]
    public async Task CommonObjectPost_WhenPayloadIsMissing_ShouldReturn400AndWarn()
    {
        // Given
        MethodInfo action = FindAction(actionName: "CommonObjectController.Post");

        // When
        Outcome outcome = await Invoke(action: action, failure: null, returnNull: true, nullArguments: true);

        // Then
        AssertDirectClientError(outcome: outcome, expectedStatus: 400);
    }

    [Theory]
    [MemberData(nameof(Actions))]
    public async Task Action_WhenDependencyThrowsValidation_ShouldReturn400AndWarn(MethodInfo action)
    {
        // Given
        Type exceptionType = ProductAssembly.GetType("cCoder.ContentManagement.Models.Exceptions.ContentManagementValidationException")!;
        Exception exception = (Exception)Activator.CreateInstance(exceptionType, new Exception("Sensitive validation detail."))!;

        // When / Then
        await AssertFailure(action: action, failure: exception, expectedStatus: 400, warningCount: 1, errorCount: 0);
    }

    [Theory]
    [MemberData(nameof(Actions))]
    public Task Action_WhenDependencyThrowsAuthentication_ShouldReturn401AndWarn(MethodInfo action) =>
        AssertFailure(action: action, failure: new AuthenticationException("Sensitive authentication detail."), expectedStatus: 401, warningCount: 1, errorCount: 0);

    [Theory]
    [MemberData(nameof(Actions))]
    public Task Action_WhenDependencyThrowsUnauthorizedAccess_ShouldReturn403AndWarn(MethodInfo action) =>
        AssertFailure(action: action, failure: new UnauthorizedAccessException("Sensitive authorization detail."), expectedStatus: 403, warningCount: 1, errorCount: 0);

    [Theory]
    [MemberData(nameof(Actions))]
    public Task Action_WhenDependencyThrowsUnexpected_ShouldReturnDetailFree500AndLogError(MethodInfo action) =>
        AssertFailure(action: action, failure: new InvalidOperationException("Sensitive unexpected detail."), expectedStatus: 500, warningCount: 0, errorCount: 1);

    [Theory]
    [MemberData(nameof(Actions))]
    public void Action_ShouldOwnClassifiedExceptionCatches(MethodInfo action)
    {
        // Given
        MethodInfo implementation = action.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType
            .GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic) ?? action;

        // When
        string[] catches = implementation.GetMethodBody()!.ExceptionHandlingClauses
            .Where(clause => clause.Flags == ExceptionHandlingClauseOptions.Clause)
            .Select(clause => clause.CatchType!.FullName!)
            .ToArray();

        // Then
        Assert.Contains(expected: typeof(ContentManagementValidationException).FullName!, collection: catches);
        Assert.Contains(expected: typeof(AuthenticationException).FullName!, collection: catches);
        Assert.Contains(expected: typeof(UnauthorizedAccessException).FullName!, collection: catches);
        Assert.Contains(expected: typeof(SecurityException).FullName!, collection: catches);
        Assert.Contains(expected: typeof(Exception).FullName!, collection: catches);
    }

    [Fact]
    public void Inventory_ShouldContainEveryControllerAction()
    {
        // Given / When
        int actionCount = Actions.Count();

        // Then
        Assert.Equal(expected: 81, actual: actionCount);
    }

    private static bool IsAction(MethodInfo method)
    {
        Type returnType = method.ReturnType;
        if (typeof(IActionResult).IsAssignableFrom(c: returnType))
        {
            return true;
        }

        if (!returnType.IsGenericType)
        {
            return false;
        }

        Type resultType = returnType.GetGenericArguments()[0];

        return typeof(IActionResult).IsAssignableFrom(c: resultType)
            || resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ActionResult<>);
    }

    private static bool IsHappyGap(MethodInfo action)
    {
        string actionName = $"{action.DeclaringType!.Name}.{action.Name}";

        return actionName is
            "AppController.GetIsAdmin" or
            "AppCultureController.GetAll" or "AppCultureController.DeleteAll" or
            "AppUserController.GetUsers" or
            "CommonObjectController.GetLatest" or "CommonObjectController.GetAll" or
            "ComponentController.GetAll" or
            "ContentController.GetAll" or
            "CultureController.GetAll" or
            "LayoutController.GetAll" or
            "PackageController.PostImportAsync" or
            "PageController.GetRootFor" or "PageController.GetMenu" or
            "PageInfoController.GetAll" or
            "PageRenderCacheController.Get" or "PageRenderCacheController.Post" or
            "PageRenderCacheController.Put" or "PageRenderCacheController.Delete" or
            "PageRoleController.GetAll" or "PageRoleController.DeleteAll" or
            "RenderController.PostTemplate" or "RenderController.GetComponent" or
            "ScriptController.GetAll" or
            "SubmissionController.GetAll" or
            "TemplateContentController.PostHtmlToPdfAsync" or
            "TemplateController.GetAll";
    }

    private static MethodInfo FindAction(string actionName)
    {
        string[] signature = actionName.Split('/');
        string[] parts = signature[0].Split('.');
        int? parameterCount = signature.Length == 2 ? int.Parse(signature[1]) : null;

        return Actions
            .Select(data => (MethodInfo)data[0])
            .Single(action => action.DeclaringType!.Name == parts[0]
                && action.Name == parts[1]
                && (parameterCount is null || action.GetParameters().Length == parameterCount)
                && (actionName != "PageController.Get"
                    || action.GetParameters()[0].ParameterType == typeof(int)));
    }

    private static void AssertDirectClientError(Outcome outcome, int expectedStatus)
    {
        Assert.True(
            condition: expectedStatus == (outcome.Result as IStatusCodeActionResult)?.StatusCode,
            userMessage: $"Expected {expectedStatus}; warning was {outcome.Logging.LastWarning}.");

        Assert.Equal(expected: 0, actual: outcome.Logging.InformationCount);
        Assert.Equal(expected: 1, actual: outcome.Logging.WarningCount);
        Assert.Equal(expected: 0, actual: outcome.Logging.ErrorCount);
    }

    private static async Task AssertFailure(MethodInfo action, Exception failure, int expectedStatus, int warningCount, int errorCount)
    {
        // Given / When
        Outcome outcome = await Invoke(action: action, failure: failure);

        // Then
        Assert.Equal(expected: expectedStatus, actual: (outcome.Result as IStatusCodeActionResult)?.StatusCode);
        Assert.Equal(expected: 0, actual: outcome.Logging.InformationCount);
        Assert.Equal(expected: warningCount, actual: outcome.Logging.WarningCount);
        Assert.Equal(expected: errorCount, actual: outcome.Logging.ErrorCount);
        string payload = (outcome.Result as ObjectResult)?.Value?.ToString() ?? string.Empty;
        Assert.DoesNotContain(expectedSubstring: "Sensitive", actualString: payload, comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<Outcome> Invoke(
        MethodInfo action,
        Exception failure,
        bool invalidModelState = false,
        bool returnNull = false,
        bool nullArguments = false)
    {
        ConstructorInfo constructor = action.DeclaringType!
            .GetConstructors()
            .Single();

        ContractProxy logging = null;

        object[] dependencies = constructor
            .GetParameters()
            .Select(parameter =>
            {
                bool isLogging = parameter.ParameterType.FullName == "cCoder.ContentManagement.Brokers.Loggings.ILoggingBroker";
                ContractProxy proxy = CreateProxy(type: parameter.ParameterType, failure: isLogging ? null : failure);
                proxy.ReturnNull = !isLogging && returnNull;

                if (isLogging)
                {
                    logging = proxy;
                }

                return (object)proxy;
            })
            .ToArray();

        object controller = constructor.Invoke(parameters: dependencies);

        if (controller is ControllerBase controllerBase)
        {
            controllerBase.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            controllerBase.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("<html><body>test</body></html>"));

            if (invalidModelState)
            {
                controllerBase.ModelState.AddModelError(key: "contract", errorMessage: "Invalid contract input.");
            }
        }

        object[] arguments = action
            .GetParameters()
            .Select(parameter => nullArguments
                ? parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null
                : Value(type: parameter.ParameterType))
            .ToArray();

        object invocation = action.Invoke(obj: controller, parameters: arguments)!;
        IActionResult result = await Resolve(invocation: invocation);

        return new Outcome(Result: result, Logging: logging!);
    }

    private static ContractProxy CreateProxy(Type type, Exception failure)
    {
        ContractProxy proxy = (ContractProxy)DispatchProxy.Create(interfaceType: type, proxyType: typeof(ContractProxy));
        proxy.Failure = failure;
        return proxy;
    }

    private static object Value(Type type)
    {
        if (type == typeof(string))
        {
            return "dGVzdA==";
        }

        if (type == typeof(CancellationToken))
        {
            return CancellationToken.None;
        }

        if (type == typeof(int))
        {
            return 1;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            Type resultType = type.GetGenericArguments()[0];
            object result = Value(type: resultType);

            return typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(obj: null, parameters: new[] { result })!;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            Type resultType = type.GetGenericArguments()[0];
            object result = Value(type: resultType);

            return Activator.CreateInstance(type: type, args: new[] { result })!;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return Array.CreateInstance(elementType: type.GetGenericArguments()[0], length: 0);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
        {
            Type elementType = type.GetGenericArguments()[0];
            object values = Array.CreateInstance(elementType: elementType, length: 0);

            return typeof(Queryable)
                .GetMethods()
                .Single(method => method.Name == nameof(Queryable.AsQueryable)
                    && method.IsGenericMethodDefinition
                    && method.GetParameters().Length == 1)
                .MakeGenericMethod(elementType)
                .Invoke(obj: null, parameters: new[] { values })!;
        }

        if (type.IsValueType)
        {
            return Activator.CreateInstance(type)!;
        }

        if (type.IsArray)
        {
            return Array.CreateInstance(elementType: type.GetElementType()!, length: 0);
        }

        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<IActionResult> Resolve(object invocation)
    {
        if (invocation is IActionResult result)
        {
            return result;
        }

        Task task;

        if (invocation is Task directTask)
        {
            task = directTask;
        }
        else
        {
            task = (Task)invocation
                .GetType()
                .GetMethod("AsTask")!
                .Invoke(obj: invocation, parameters: null)!;
        }

        await task;

        return (IActionResult)task
            .GetType()
            .GetProperty("Result")!
            .GetValue(obj: task)!;
    }

    public class ContractProxy : DispatchProxy
    {
        public Exception Failure { get; set; }
        public bool ReturnNull { get; set; }
        public int InformationCount { get; private set; }
        public int WarningCount { get; private set; }
        public int ErrorCount { get; private set; }
        public Exception LastWarning { get; private set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.Name == "LogInformation")
            {
                InformationCount++;
                return null;
            }

            if (targetMethod.Name == "LogWarning")
            {
                WarningCount++;

                LastWarning = args
                    .OfType<Exception>()
                    .FirstOrDefault();

                return null;
            }

            if (targetMethod.Name == "LogError")
            {
                ErrorCount++;
                return null;
            }

            if (Failure is not null)
            {
                throw Failure;
            }

            if (ReturnNull)
            {
                if (targetMethod.Name == "LatestCommonObjects")
                {
                    return null;
                }

                if (targetMethod.Name == "RenderComponentRenderResultAsync")
                {
                    Type modelType = ProductAssembly.GetType("cCoder.ContentManagement.Models.ComponentRenderResult")!;
                    object model = Activator.CreateInstance(modelType)!;

                    return Activator.CreateInstance(type: targetMethod.ReturnType, args: new[] { model })!;
                }

                if (targetMethod.ReturnType.IsValueType && !targetMethod.ReturnType.IsGenericType)
                {
                    return Value(type: targetMethod.ReturnType);
                }

                if (targetMethod.ReturnType.IsGenericType
                    && targetMethod.ReturnType.GetGenericTypeDefinition() is Type collectionType
                    && (collectionType == typeof(IEnumerable<>) || collectionType == typeof(IQueryable<>)))
                {
                    return Value(type: targetMethod.ReturnType);
                }

                if (targetMethod.ReturnType.IsGenericType
                    && targetMethod.ReturnType.GetGenericTypeDefinition() is Type taskType
                    && (taskType == typeof(Task<>) || taskType == typeof(ValueTask<>)))
                {
                    Type resultType = targetMethod.ReturnType.GetGenericArguments()[0];

                    object result = resultType.IsValueType
                        || resultType.IsGenericType
                        && resultType.GetGenericTypeDefinition() is Type resultCollectionType
                        && (resultCollectionType == typeof(IEnumerable<>) || resultCollectionType == typeof(IQueryable<>))
                            ? Value(type: resultType)
                            : null;


                    return taskType == typeof(Task<>)
                        ? typeof(Task)
                            .GetMethod(nameof(Task.FromResult))!
                            .MakeGenericMethod(resultType)
                            .Invoke(null, new[] { result })!
                        : Activator.CreateInstance(targetMethod.ReturnType, new[] { result })!;
                }

                return null;
            }

            if (targetMethod.Name is "RenderTemplateRenderResultAsync" or "RenderComponentRenderResultAsync")
            {
                string modelName = targetMethod.Name.StartsWith("RenderTemplate", StringComparison.Ordinal)
                    ? "TemplateRenderResult"
                    : "ComponentRenderResult";

                Type modelType = ProductAssembly.GetType($"cCoder.ContentManagement.Models.{modelName}")!;
                object model = Activator.CreateInstance(modelType)!;
                modelType.GetProperty("Content")!.SetValue(obj: model, value: "test");

                return Activator.CreateInstance(type: targetMethod.ReturnType, args: new[] { model })!;
            }

            return Value(type: targetMethod.ReturnType);
        }
    }

    private sealed record Outcome(IActionResult Result, ContractProxy Logging);
}