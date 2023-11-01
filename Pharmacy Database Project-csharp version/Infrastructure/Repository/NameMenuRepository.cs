﻿using AutoMapper;
using Domain.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class NameMenuRepository : INameMenuRepository
{
    private readonly PharmacyDbContext _context;
    private readonly IMapper _mapper;
    
    public NameMenuRepository(PharmacyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public IEnumerable<string> GetAllDistinctManufacturers()
    {
        return _context.NameMenus
            .Select(nm => nm.ProductManufacturer)
            .Distinct()
            .ToList();
    }
    
    public async Task UpdateManufacturerNameAsync(string oldName, string newName)
    {
        var productsToUpdate = await _context.NameMenus
            .Where(nm => nm.ProductManufacturer == oldName)
            .ToListAsync();

        productsToUpdate.ForEach(nm => nm.ProductManufacturer = newName);
        await _context.SaveChangesAsync();
    }
}