using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;

namespace Quiz.Exam.Web.Application.Commands.UserCommands
{



    public record UpdateUserRolePermissionsCommand(
    UserId UserId,
    RoleId RoleId,
    IEnumerable<string> PermissionCodes) : ICommand;

    public class UpdateUserRolePermissionsCommandHandler(IUserRepository userRepository)
        : ICommandHandler<UpdateUserRolePermissionsCommand>
    {
        public async Task Handle(UpdateUserRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var adminUser = await userRepository.GetAsync(request.UserId, cancellationToken) ??
                            throw new KnownException($"用户不存在，AdminUserId={request.UserId}");

            var permissions = request.PermissionCodes.Select(code => new UserPermission(code, request.RoleId));

            adminUser.UpdateRolePermissions(request.RoleId, permissions);
        }
    }
}
