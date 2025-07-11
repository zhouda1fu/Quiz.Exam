using FluentValidation;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using Quiz.Exam.Web.Application.Queries;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Application.Commands;

public record CreateUserCommand(string Username, string Email, string Password) : ICommand<UserId>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(UserQuery userQuery)
    {
        RuleFor(u => u.Username).NotEmpty().WithMessage("用户名不能为空");
        RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("邮箱格式不正确");
        RuleFor(u => u.Password).NotEmpty().WithMessage("密码不能为空");
        RuleFor(u => u.Username).MustAsync(async (n, ct) => !await userQuery.DoesUserExist(n, ct))
            .WithMessage(u => $"该用户已存在，Username={u.Username}");
        RuleFor(u => u.Email).MustAsync(async (e, ct) => !await userQuery.DoesEmailExist(e, ct))
            .WithMessage(u => $"该邮箱已存在，Email={u.Email}");
    }
}

public class CreateUserCommandHandler(IUserRepository userRepository) : ICommandHandler<CreateUserCommand, UserId>
{
    public async Task<UserId> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 哈希密码
        var passwordHash = PasswordHasher.HashPassword(request.Password);

        // 创建用户
        var user = new User(request.Username, request.Email, passwordHash);
        await userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }

} 