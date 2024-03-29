using AutoMapper;
using Domain.Models;
using Domain.Repositories.Interface;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class CustomerRepository : Repository<Customer, CustomerDomain, int>, ICustomerRepository
{
    private readonly PharmacyDbContext _context;
    private readonly IMapper _mapper;

    public CustomerRepository(PharmacyDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<double> CalculateTotalDebtByIdAsync(string customerId)
    {
        return await _context.Customers
            .Where(c => c.CustomerNID == customerId)
            .Select(c => c.Debt)
            .SumAsync();
    }

    public async Task<IEnumerable<CustomerDomain>> FindWithOrdersAsync()
    {
        var customers = await _context.Customers
            .Where(c => c.CustomerOrders.Any())
            .ToListAsync();
        return _mapper.Map<List<CustomerDomain>>(customers);
    }


    public async Task UpdatePhoneAsync(string oldPhoneNumber, string newPhoneNumber)
    {
        var customerPhone = await _context.CustomerPhones
            .FirstOrDefaultAsync(cp => cp.Phone == oldPhoneNumber);

        if (customerPhone != null)
        {
            customerPhone.Phone = newPhoneNumber;
            await _context.SaveChangesAsync();
        }
    }
}