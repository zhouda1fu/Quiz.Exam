using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;

namespace Quiz.Exam.Web.Application.IntegrationEventHandlers
{
    public record OrderPaidIntegrationEvent(OrderId OrderId);
}
