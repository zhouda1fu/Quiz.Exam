using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure.Repositories;

namespace Quiz.Exam.Web.Application.Commands.UserCommands
{


    public record UpdateUserRoleInfoCommand(UserId UserId, RoleId RoleId, string RoleName) : ICommand;

    public class UpdateUserRoleInfoCommandHandler(IUserRepository userRepository)
        : ICommandHandler<UpdateUserRoleInfoCommand>
    {
        public async Task Handle(UpdateUserRoleInfoCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken) ??
                       throw new KnownException($"未找到用户，AdminUserId = {request.UserId}");

            user.UpdateRoleInfo(request.RoleId, request.RoleName);
        }
    }
}
