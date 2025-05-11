namespace pja_apbd_cwic9.DTOs;

public class GetOrder
{
    public int IdOrder { set; get; }
    public int IdProduct { set; get; }
    public int Amount { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? FulfilledAt { set; get; }
}