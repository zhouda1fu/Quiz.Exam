using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Const;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;


[Tags("Roles")]
public class DeleteRoleEndpoint : EndpointWithoutRequest<ResponseData<bool>>
{
    private readonly IMediator _mediator;

    public DeleteRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/roles/{roleId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(AppPermissions.UserDelete);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roleId = Route<long>("roleId");
        var command = new DeleteRoleCommand(new RoleId(roleId));
        await _mediator.Send(command, ct);
        await SendAsync(true.AsResponseData(), cancellation: ct);
    }
}