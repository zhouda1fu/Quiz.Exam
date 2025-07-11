using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.DomainEvents.RoleEvents;

public record RoleDeactivatedDomainEvent(Role Role) : IDomainEvent; 