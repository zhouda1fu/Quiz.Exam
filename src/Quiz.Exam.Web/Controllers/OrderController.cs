﻿using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.IntegrationEventHandlers;
using Quiz.Exam.Web.Application.Queries;
using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.Extensions.Dto;

namespace Quiz.Exam.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(IMediator mediator, OrderQuery orderQuery, ICapPublisher capPublisher) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }

    [HttpPost]
    public async Task<ResponseData<OrderId>> Post([FromBody] CreateOrderRequest request)
    {
        var cmd = new CreateOrderCommand(request.Name, request.Price, request.Count);
        var id = await mediator.Send(cmd);
        return id.AsResponseData();
    }


    [HttpGet]
    [Route("/get/{id}")]
    public async Task<ResponseData<Order?>> GetById([FromRoute] OrderId id)
    {
        var order = await orderQuery.QueryOrder(id, HttpContext.RequestAborted).AsResponseData();
        return order;
    }


    [HttpGet]
    [Route("/sendEvent")]
    public async Task SendEvent(OrderId id)
    {
        await capPublisher.PublishAsync("OrderPaidIntegrationEvent", new OrderPaidIntegrationEvent(id));
    }
}

public record CreateOrderRequest(string Name, int Price, int Count);