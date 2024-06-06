namespace Pharmacy.API.Dtos;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? Email { get; set; }
    public double Dues { get; set; }
}