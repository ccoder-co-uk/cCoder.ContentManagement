// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IContentEventBroker
{
    ValueTask RaiseContentAddEventAsync(EventMessage<Content> message);

    ValueTask RaiseContentUpdateEventAsync(EventMessage<Content> message);

    ValueTask RaiseContentDeleteEventAsync(EventMessage<Content> message);
}