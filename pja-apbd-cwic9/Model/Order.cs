namespace pja_apbd_cwic9.Model;

public class Order
{
    public int IdOrder { set; get; }
    public int IdProduct { set; get; }
    public int Amount { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? FulfilledAt { set; get; }
}