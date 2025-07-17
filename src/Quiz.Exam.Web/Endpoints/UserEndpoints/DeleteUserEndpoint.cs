using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Const;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record DeleteUserRequest(UserId UserId);

[Tags("Users")]
public class DeleteUserEndpoint : Endpoint<DeleteUserRequest, ResponseData<bool>>
{
    private readonly IMediator _mediator;

    public DeleteUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/users/delete");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(AppPermissions.UserDelete);
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        var command = new DeleteUserCommand(req.UserId);
        await _mediator.Send(command, ct);
        
        await SendAsync(true.AsResponseData(), cancellation: ct);
    }
} 