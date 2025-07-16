using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Commands.UserCommands;

public record DeleteUserCommand(UserId UserId) : ICommand;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("用户ID不能为空");
    }
}

public class DeleteUserCommandHandler(IUserRepository userRepository) 
    : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KnownException("用户不存在");
        }

        // 检查是否为管理员用户，防止删除管理员
        if (user.Name.ToLower() == "admin")
        {
            throw new KnownException("不能删除管理员用户");
        }
        user.Delete();
    }
} 