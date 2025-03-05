using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiTenantSample.Entities;
using MultiTenantSample.Models;
using MultiTenantSample.Services;

namespace MultiTenantSample.Controllers;

/// <summary>
/// 订单控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="orderService">订单服务</param>
    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }
    
    /// <summary>
    /// 获取当前租户的所有订单
    /// </summary>
    /// <returns>订单列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        var orders = await _orderService.GetOrdersForCurrentTenantAsync();
        return Ok(orders);
    }
    
    /// <summary>
    /// 获取当前租户的指定订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单信息</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(long id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();
            
        return Ok(order);
    }
    
    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="createOrderDto">创建订单数据</param>
    /// <returns>创建的订单</returns>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        try
        {
            var id = await _orderService.CreateOrderAsync(createOrderDto);
            var createdOrder = await _orderService.GetOrderByIdAsync(id);
            return CreatedAtAction(nameof(GetOrder), new { id }, createdOrder);
        }
        catch (System.ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="status">订单状态</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(long id, OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
    
    /// <summary>
    /// 删除订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(long id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
            return NotFound("订单不存在");
            
        return NoContent();
    }
}
