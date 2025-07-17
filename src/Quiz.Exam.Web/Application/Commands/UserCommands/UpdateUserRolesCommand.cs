using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using Quiz.Exam.Web.Application.Queries;

namespace Quiz.Exam.Web.Application.Commands.UserCommands
{


    public record UpdateUserRolesCommand(UserId UserId, List<AssignAdminUserRoleQueryDto> RolesToBeAssigned)
    : ICommand;

    public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
    {
        public UpdateUserRolesCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class UpdateUserRolesCommandHandler(IUserRepository userRepository)
        : ICommandHandler<UpdateUserRolesCommand>
    {
        public async Task Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var adminUser = await userRepository.GetAsync(request.UserId, cancellationToken)
                            ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

            List<UserRole> roles = [];
            List<UserPermission> permissions = [];

            foreach (var assignAdminUserRoleDto in request.RolesToBeAssigned)
            {
                roles.Add(new UserRole(assignAdminUserRoleDto.RoleId, assignAdminUserRoleDto.RoleName));
                permissions.AddRange(assignAdminUserRoleDto.PermissionCodes.Select(code =>
                    new UserPermission(code, assignAdminUserRoleDto.RoleId)));
            }

            adminUser.UpdateRoles(roles, permissions);
        }
    }
}
