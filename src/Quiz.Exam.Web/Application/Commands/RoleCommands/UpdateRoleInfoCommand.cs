using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using Quiz.Exam.Web.Application.Queries;

namespace Quiz.Exam.Web.Application.Commands.RoleCommands
{
    public record UpdateRoleInfoCommand(RoleId RoleId, string Name, string Description, 
        IEnumerable<string> PermissionCodes) : ICommand;

    public class UpdateRoleInfoCommandValidator : AbstractValidator<UpdateRoleInfoCommand>
    {
        public UpdateRoleInfoCommandValidator(RoleQuery roleQuery)
        {
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class UpdateRoleInfoCommandHandler(IRoleRepository roleRepository) : ICommandHandler<UpdateRoleInfoCommand>
    {
        public async Task Handle(UpdateRoleInfoCommand request, CancellationToken cancellationToken)
        {
            var role = await roleRepository.GetAsync(request.RoleId, cancellationToken) ??
                       throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
            role.UpdateRoleInfo(request.Name, request.Description);

            // 更新角色权限
            var permissions = request.PermissionCodes.Select(perm => new RolePermission(perm));
            role.UpdateRolePermissions(permissions);
        }
    }
}
