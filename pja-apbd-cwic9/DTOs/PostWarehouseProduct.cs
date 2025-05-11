using System.ComponentModel.DataAnnotations;

namespace pja_apbd_cwic9.DTOs;

public class PostWarehouseProduct
{
    [Required] [Range(1, int.MaxValue)] public int IdProduct { set; get; }

    [Required] [Range(1, int.MaxValue)] public int IdWarehouse { set; get; }

    [Required] public int Amount { set; get; }

    [Required] public DateTime CreatedAt { set; get; }
}