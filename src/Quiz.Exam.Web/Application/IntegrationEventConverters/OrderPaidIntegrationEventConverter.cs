using Quiz.Exam.Domain.DomainEvents;
using Quiz.Exam.Web.Application.IntegrationEventHandlers;
using NetCorePal.Extensions.DistributedTransactions;

namespace Quiz.Exam.Web.Application.IntegrationEventConverters;

public class OrderPaidIntegrationEventConverter
    : IIntegrationEventConverter<OrderPaidDomainEvent, OrderPaidIntegrationEvent>
{
    public OrderPaidIntegrationEvent Convert(OrderPaidDomainEvent domainEvent)
    {
        return new OrderPaidIntegrationEvent(domainEvent.Order.Id);
    }
}