using pja_apbd_cwic9.DTOs;

namespace pja_apbd_cwic9.Services;

public interface IDbService
{
    public Task<GetProduct> GetProductByIdAsync(int id);
    public Task<GetWarehouse> GetWarehouseByIdAsync(int id);
    public Task<GetOrder> GetOrderByAmountAndProductIdAsync(int amount, int productId, DateTime createdAt);
    public Task<GetWarehouseProduct> GetWarehouseProductByOrderIdAsync(int id);
    public Task UpdateOrderFulfilledDateAsync(int id, DateTime date);
    public Task<int> InsertProductWarehouseAsync(PostWarehouseProduct request, int idOrder);
    public Task<int> InsertByProcedureAsync(PostWarehouseProduct request);
}