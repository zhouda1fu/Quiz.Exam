using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.DomainEvents;

public record OrderPaidDomainEvent(Order Order) : IDomainEvent;