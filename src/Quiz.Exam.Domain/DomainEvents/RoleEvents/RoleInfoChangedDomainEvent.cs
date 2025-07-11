using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.DomainEvents.RoleEvents;

public record RoleInfoChangedDomainEvent(Role Role) : IDomainEvent; 