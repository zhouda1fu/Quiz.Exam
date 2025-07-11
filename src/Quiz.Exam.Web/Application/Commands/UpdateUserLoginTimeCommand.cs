using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Commands;

public record UpdateUserLoginTimeCommand(UserId UserId, DateTimeOffset LoginTime) : ICommand;

public class UpdateUserLoginTimeCommandValidator : AbstractValidator<UpdateUserLoginTimeCommand>
{
    public UpdateUserLoginTimeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("用户ID不能为空");
        RuleFor(x => x.LoginTime).NotEmpty().WithMessage("登录时间不能为空");
    }
}

public class UpdateUserLoginTimeCommandHandler(IUserRepository userRepository) : ICommandHandler<UpdateUserLoginTimeCommand>
{
    public async Task Handle(UpdateUserLoginTimeCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KnownException($"用户不存在，UserId={request.UserId}");
        }

        user.UpdateLastLoginTime(request.LoginTime);
    }
} 