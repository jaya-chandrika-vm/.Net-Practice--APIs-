using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> AddOrderAsync(Order order);
    Task<Order?> GetOrderByIdAsync(int id);
    // Task<IEnumerable<Order>> GetAllOrdersAsync();
}