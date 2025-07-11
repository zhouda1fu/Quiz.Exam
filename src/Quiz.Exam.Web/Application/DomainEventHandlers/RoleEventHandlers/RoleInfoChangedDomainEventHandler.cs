using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Exam.Domain.DomainEvents.RoleEvents;
using Quiz.Exam.Web.Application.Commands;
using NetCorePal.Extensions.Domain;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Application.Commands.UserCommands;

namespace Quiz.Exam.Web.Application.DomainEventHandlers.RoleEventHandlers;


public class RoleInfoChangedDomainEventHandler(IMediator mediator, UserQuery userQuery)
    : IDomainEventHandler<RoleInfoChangedDomainEvent>
{
    public async Task Handle(RoleInfoChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var role = notification.Role;
        var adminUserIds = await userQuery.GetUserIdsByRoleIdAsync(role.Id, cancellationToken);
        foreach (var adminUserId in adminUserIds)
            await mediator.Send(new UpdateUserRoleInfoCommand(adminUserId, role.Id, role.Name),
                cancellationToken);
    }
}