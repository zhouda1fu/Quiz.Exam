using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Commands;

public record AssignUserRolesCommand(UserId UserId, List<RoleId> RoleIds) : ICommand;

public class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("用户ID不能为空");
        RuleFor(x => x.RoleIds).NotEmpty().WithMessage("角色ID列表不能为空");
    }
}

public class AssignUserRolesCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository) 
    : ICommandHandler<AssignUserRolesCommand>
{
    public async Task Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        // 获取用户
        var user = await userRepository.GetAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KnownException("用户不存在");
        }

        // 获取角色
        var roles = new List<Role>();
        foreach (var roleId in request.RoleIds)
        {
            var role = await roleRepository.GetAsync(roleId, cancellationToken);
            if (role == null)
            {
                throw new KnownException($"角色ID {roleId} 不存在");
            }
            if (!role.IsActive)
            {
                throw new KnownException($"角色 '{role.Name}' 已被停用");
            }
            roles.Add(role);
        }

        // 创建用户角色关系
        var userRoles = roles.Select(r => new UserRole(r.Id, r.Name)).ToList();
        
        // 创建用户权限关系（从角色继承）
        var userPermissions = new List<UserPermission>();
        foreach (var role in roles)
        {
            foreach (var permission in role.Permissions)
            {
                userPermissions.Add(new UserPermission(permission.PermissionCode, role.Id));
            }
        }

        // 更新用户角色和权限
        user.UpdateRoles(userRoles, userPermissions);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
} 