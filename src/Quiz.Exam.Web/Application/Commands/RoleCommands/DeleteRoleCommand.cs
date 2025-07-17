using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

namespace Quiz.Exam.Web.Application.Commands.UserCommands;

public record DeleteRoleCommand(RoleId RoleId) : ICommand;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class DeleteRoleCommandHandler(IRoleRepository roleRepository) 
    : ICommandHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new KnownException("角色不存在");
        }

        // 检查是否为管理员用户，防止删除管理员
        if (role.Name.ToLower() == "admin")
        {
            throw new KnownException("不能删除管理员角色");
        }
        role.Delete();
    }
} 