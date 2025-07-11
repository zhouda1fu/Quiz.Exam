using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using Quiz.Exam.Web.Application.Queries;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Commands;

public record CreateRoleCommand(string Name, string Description) : ICommand<RoleId>;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleQuery roleQuery)
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage("角色名称不能为空");
        RuleFor(r => r.Description).MaximumLength(200).WithMessage("角色描述长度不能超过200个字符");
        RuleFor(r => r.Name).MustAsync(async (n, ct) => !await roleQuery.DoesRoleExist(n, ct))
            .WithMessage(r => $"该角色已存在，Name={r.Name}");
    }
}

public class CreateRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<CreateRoleCommand, RoleId>
{
    public async Task<RoleId> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // 创建角色
        var role = new Role(request.Name, request.Description);
        await roleRepository.AddAsync(role, cancellationToken);

        return role.Id;
    }
} 