using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class NameManufacturer
{
    [Key] public int ProductId { get; set; }

    [Required] [StringLength(32)] public string ProductName { get; set; }

    [Required] [StringLength(32)] public string ProductManufacturer { get; set; }

    public Product Product { get; set; }
}