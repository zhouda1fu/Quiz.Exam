using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.DomainEvents.UserEvents;

/// <summary>
/// 用户登录时间变更领域事件
/// </summary>
public record UserLoginTimeChangedDomainEvent(UserId UserId, DateTimeOffset LoginTime) : IDomainEvent; 