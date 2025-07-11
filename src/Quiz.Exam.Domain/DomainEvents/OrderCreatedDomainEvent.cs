using NetCorePal.Extensions.Domain;
using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;

namespace Quiz.Exam.Domain.DomainEvents
{
    public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;
}
