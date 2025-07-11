using Quiz.Exam.Domain.DomainEvents;
using Quiz.Exam.Web.Application.Commands;
using MediatR;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Web.Application.DomainEventHandlers
{
    internal class OrderCreatedDomainEventHandler(IMediator mediator) : IDomainEventHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            return mediator.Send(new DeliverGoodsCommand(notification.Order.Id), cancellationToken);
        }
    }
}