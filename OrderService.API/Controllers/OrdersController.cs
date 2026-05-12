using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    // [HttpGet]
    // public async Task<IActionResult> Get()
    // {
    //     var orders = await _orderRepository.GetAllOrdersAsync();
    //     return Ok(orders);
    // }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
        var createdOrder = await _orderRepository.AddOrderAsync(order);
        return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
    }
}