using System.Data;
using Microsoft.Data.SqlClient;
using pja_apbd_cwic9.DTOs;

namespace pja_apbd_cwic9.Services;

public class WarehouseProductsService : IDbService
{
    private readonly string _connectionString;

    public WarehouseProductsService(IConfiguration cfg)
    {
        _connectionString = cfg.GetConnectionString("Default");
    }

    public async Task<GetProduct> GetProductByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "SELECT * FROM Product p WHERE p.IdProduct = @id",
            connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new GetProduct
            {
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                Price = reader.GetDecimal("Price")
            };

        throw new KeyNotFoundException($"No product found with id = {id}");
    }

    public async Task<GetWarehouse> GetWarehouseByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "SELECT * FROM Warehouse p WHERE p.IdWarehouse = @id",
            connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new GetWarehouse
            {
                Name = reader.GetString("Name"),
                Address = reader.GetString("Address")
            };

        throw new KeyNotFoundException($"No warehouse found with id = {id}");
    }

    public async Task<GetOrder> GetOrderByAmountAndProductIdAsync(int amount, int productId, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "SELECT * FROM [Order] p WHERE p.IdProduct = @id AND p.Amount = @amount AND p.CreatedAt < @date",
            connection);

        command.Parameters.AddWithValue("@id", productId);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@date", createdAt);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new GetOrder
            {
                IdOrder = reader.GetInt32(0),
                IdProduct = reader.GetInt32("IdProduct"),
                Amount = reader.GetInt32("Amount"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                FulfilledAt = await reader.IsDBNullAsync("FulfilledAt") ? null : reader.GetDateTime("FulfilledAt")
            };

        throw new KeyNotFoundException(
            $"No order found with productId = {productId} and amount = {amount} or created before {createdAt}");
    }

    public async Task<GetWarehouseProduct> GetWarehouseProductByOrderIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "SELECT * FROM Product_Warehouse p WHERE p.IdProduct = @id",
            connection);

        command.Parameters.AddWithValue("@id", id);


        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new GetWarehouseProduct
            {
                IdWarehouse = reader.GetInt32("IdWarehouse"),
                IdProduct = reader.GetInt32("IdProduct"),
                IdOrder = reader.GetInt32("IdOrder"),
                Amount = reader.GetInt32("Amount"),
                Price = reader.GetDecimal("Price"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };

        throw new KeyNotFoundException($"No warehouse_product found with id = {id}");
    }

    public async Task UpdateOrderFulfilledDateAsync(int id, DateTime date)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "UPDATE [Order] SET FulfilledAt = @date OUTPUT INSERTED.IdOrder WHERE IdOrder = @id",
            connection);

        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@date", date);

        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new KeyNotFoundException($"No order found with id = {id}");
    }

    public async Task<int> InsertProductWarehouseAsync(PostWarehouseProduct request, int idOrder)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            """
            INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
            OUTPUT INSERTED.IdProductWarehouse
            VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @date)
            """,
            connection);

        var product = await GetProductByIdAsync(request.IdProduct);

        command.Parameters.AddWithValue("@idWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@idProduct", request.IdProduct);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        command.Parameters.AddWithValue("@amount", request.Amount);
        command.Parameters.AddWithValue("@price", product.Price * request.Amount);
        command.Parameters.AddWithValue("@date", DateTime.Now);

        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new KeyNotFoundException($"Couldn't insert {request}");

        return (int) result;
    }

    public async Task<int> InsertByProcedureAsync(PostWarehouseProduct request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("AddProductToWarehouse", connection){
            CommandType = CommandType.StoredProcedure
        };
        
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new Exception("Insertion failed");

        return (int) (decimal) result;
    }
}