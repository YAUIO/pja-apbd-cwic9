namespace pja_apbd_cwic9.DTOs;

public class GetWarehouseProduct
{
    public int IdWarehouse { set; get; }
    public int IdProduct { set; get; }
    public int IdOrder { set; get; }
    public int Amount { set; get; }
    public decimal Price { set; get; }
    public DateTime CreatedAt { set; get; }
}