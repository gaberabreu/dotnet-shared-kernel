using MediatR;

namespace SharedKernel;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent;