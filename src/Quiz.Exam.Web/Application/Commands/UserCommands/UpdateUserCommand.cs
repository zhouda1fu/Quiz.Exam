using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using Quiz.Exam.Web.Application.Queries;

namespace Quiz.Exam.Web.Application.Commands.UserCommands
{


    public record UpdateUserCommand(UserId UserId, string Name, string Email, string Password, string Phone, string RealName, int Status) : ICommand<UserId>;

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator(UserQuery userQuery)
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("邮箱格式不正确");
            RuleFor(u => u.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(u => u.Name).MustAsync(async (n, ct) => !await userQuery.DoesUserExist(n, ct))
                .WithMessage(u => $"该用户已存在，Name={u.Name}");
            RuleFor(u => u.Email).MustAsync(async (e, ct) => !await userQuery.DoesEmailExist(e, ct))
                .WithMessage(u => $"该邮箱已存在，Email={u.Email}");
        }
    }

    public class UpdateUserCommandHandler(IUserRepository userRepository) : ICommandHandler<UpdateUserCommand, UserId>
    {
        public async Task<UserId> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new KnownException($"用户不存在，UserId={request.UserId}");
            }

            user.UpdateUserInfo(request.Name, request.Phone,
                request.Password,
                request.RealName, request.Status, request.Email);
            return user.Id;
        }

    }
}
