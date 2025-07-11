using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.DomainEvents.UserEvents;

public record UserDeactivatedDomainEvent(User User) : IDomainEvent; 