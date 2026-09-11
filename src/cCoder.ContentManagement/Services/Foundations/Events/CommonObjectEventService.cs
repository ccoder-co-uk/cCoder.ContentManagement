// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CommonObjectEventService(ICommonObjectEventBroker commonObjectEventBroker) : ICommonObjectEventService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject commonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectAddEventAsync(inputs: [commonObject]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = commonObjectEventBroker.GetCurrentUserId()
            },
            Data = commonObject
        };

        await commonObjectEventBroker.RaiseCommonObjectAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject commonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectUpdateEventAsync(inputs: [commonObject]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = commonObjectEventBroker.GetCurrentUserId()
            },
            Data = commonObject
        };

        await commonObjectEventBroker.RaiseCommonObjectUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject commonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectDeleteEventAsync(inputs: [commonObject]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = commonObjectEventBroker.GetCurrentUserId()
            },
            Data = commonObject
        };

        await commonObjectEventBroker.RaiseCommonObjectDeleteEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectsImportedEventAsync(
        CommonObject[] commonObjects) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectsImportedEventAsync(
            inputs: [commonObjects]);

        EventMessage<CommonObject[]> message = new EventMessage<CommonObject[]>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = commonObjectEventBroker.GetCurrentUserId()
            },
            Data = commonObjects
        };

        await commonObjectEventBroker
            .RaiseCommonObjectsImportedEventAsync(message: message);

    }, isValueTask: true);
}