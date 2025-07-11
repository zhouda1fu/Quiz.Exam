﻿using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using Quiz.Exam.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Commands;

public record class OrderPaidCommand(OrderId OrderId) : ICommand;

public class OrderPaidCommandLock : ICommandLock<OrderPaidCommand>
{
    public Task<CommandLockSettings> GetLockKeysAsync(OrderPaidCommand command,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(command.OrderId.ToCommandLockSettings());
    }
}

public class OrderPaidCommandHandler(IOrderRepository orderRepository) : ICommandHandler<OrderPaidCommand>
{
    public async Task Handle(OrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(request.OrderId, cancellationToken) ??
                    throw new KnownException($"未找到订单，OrderId = {request.OrderId}");
        order.OrderPaid();
    }
}