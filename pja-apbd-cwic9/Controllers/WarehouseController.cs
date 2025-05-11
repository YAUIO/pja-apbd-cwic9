using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using pja_apbd_cwic9.DTOs;
using pja_apbd_cwic9.Services;

namespace pja_apbd_cwic9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IDbService _service;

    public WarehouseController(IDbService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> PostWarehouseProduct(PostWarehouseProduct product)
    {
        try
        {
            await _service.GetProductByIdAsync(product.IdProduct);
            await _service.GetWarehouseByIdAsync(product.IdWarehouse);
            var order = await _service.GetOrderByAmountAndProductIdAsync(product.Amount, product.IdProduct, product.CreatedAt);
            try
            {
                await _service.GetWarehouseProductByOrderIdAsync(order.IdOrder);
                return Conflict($"Order {order} was already fulfilled");
            }
            catch (Exception _)
            {
                // continue because no warehouse_product was found
            }

            await _service.UpdateOrderFulfilledDateAsync(order.IdOrder, DateTime.Now);
            var id = await _service.InsertProductWarehouseAsync(product, order.IdOrder);
            return Created("",id);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    [Route("procedure")]
    public async Task<IActionResult> PostWarehouseProductByProcedure (PostWarehouseProduct product)
    {
        try
        {
            var id = await _service.InsertByProcedureAsync(product);
            return Created("", id);
        }
        catch (SqlException e)
        {
            if (e.Message.Contains("does not exist"))
            {
                return NotFound(e.Message);
            }
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}