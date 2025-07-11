using Quiz.Exam.Web.Application.Commands;
using DotNetCore.CAP;
using MediatR;
using NetCorePal.Extensions.DistributedTransactions;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.IntegrationEventHandlers
{
    public class OrderPaidIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<OrderPaidIntegrationEvent>
    {
        public Task HandleAsync(OrderPaidIntegrationEvent eventData, CancellationToken cancellationToken = default)
        {
            var cmd = new OrderPaidCommand(eventData.OrderId);
            return mediator.Send(cmd, cancellationToken);
        }
    }
}