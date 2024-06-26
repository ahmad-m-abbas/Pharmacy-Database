namespace Domain.Models;

public class EmployeeDomain
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NationalId { get; set; }
    public DateTime DateOfWork { get; set; }
    public double HourlyPaid { get; set; }
    public string Password { get; set; }
    public string IsManager { get; set; }
    public string IsActive { get; set; }
}