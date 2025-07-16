using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Exam.Domain.DomainEvents.RoleEvents;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Application.Commands;
using NetCorePal.Extensions.Domain;
using Quiz.Exam.Web.Const;
using Quiz.Exam.Web.Application.Commands.UserCommands;

namespace Quiz.Exam.Web.Application.DomainEventHandlers.RoleEventHandlers;

public class RolePermissionsChangedDomainEventHandler(
    IMediator mediator,
    UserQuery userQuery,
    IMemoryCache memoryCache) : IDomainEventHandler<RolePermissionChangedDomainEvent>
{
    public async Task Handle(RolePermissionChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var roleId = notification.Role.Id;
        var userIds = await userQuery.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
        var permissionCodes = notification.Role.Permissions
            .Select(p => p.PermissionCode)
            .ToArray();

        foreach (var userId in userIds)
        {
            memoryCache.Remove($"{CacheKeys.UserPermissions}:{userId}");
            await mediator.Send(new UpdateUserRolePermissionsCommand(userId, roleId, permissionCodes),
                cancellationToken);
        }
    }
}