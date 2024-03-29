﻿using System.Diagnostics;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Domain.Models;
using Domain.Repositories.Interface;
using Domain.Services.Implementations;
using Domain.Services.Interfaces;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Pharmacy.Configuration;

namespace Console_Application;

public static class ServiceConfiguration
{

    public static IServiceCollection AddConnectionStringConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new ApplicationConfiguration();
        configuration.GetSection("ApplicationConfiguration").Bind(config);

        if (config.DbConnection is null || string.IsNullOrEmpty(config.DbConnection.ConnectionString))
        {
            throw new InvalidOperationException("Database configuration is not properly loaded.");
        }

        services.AddSingleton<IPharmacyDbConnectionStringProvider>(provider => new PharmacyDbConnectionStringProvider(config));
        return services;
    }
    
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
    {
        services.AddDbContext<PharmacyDbContext>((serviceProvider, options) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IPharmacyDbConnectionStringProvider>();
            var connectionString = connectionStringProvider.GetPharmacyReadOnlyConnectionString();
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Transient);

        return services;
    }


    public static IServiceCollection AddLogging(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
        return services;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRepository<SupplierDomain, int>, Repository<Supplier, SupplierDomain, int>>();
        serviceCollection.AddScoped<IRepository<EmployeeDomain, int>, Repository<Employee, EmployeeDomain, int>>();
        serviceCollection.AddScoped<IRepository<SupplierPhone, int>, Repository<SupplierPhone, SupplierPhone, int>>();
        serviceCollection.AddScoped<IRepository<TaxDomain, string>, Repository<Tax, TaxDomain, string>>();
        serviceCollection.AddScoped<IRepository<PaymentDomain, int>, Repository<Payment, PaymentDomain, int>>();
        serviceCollection.AddScoped<IRepository<ChequeDomain, int>, Repository<Cheque, ChequeDomain, int>>();
        serviceCollection.AddScoped<IRepository<WorkHoursDomain, int>, Repository<WorkHours, WorkHoursDomain, int>>();
        serviceCollection.AddScoped<IWorkHoursService, WorkHoursService>();

        return serviceCollection;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection serviceCollection)
    {
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddExpressionMapping();

            var entityAssembly = Assembly.GetAssembly(typeof(BaseModel));
            var entityTypes = entityAssembly.GetTypes()
                .Where(t => t.Namespace == "Infrastructure.Entities");

            var modelAssembly = Assembly.GetAssembly(typeof(BaseModelDomain)); 
            var modelTypes = modelAssembly.GetTypes()
                .Where(t => t.Namespace == "Domain.Models");

            foreach (var entityType in entityTypes)
            {
                var modelName = entityType.Name + "Domain";
                var modelType = modelTypes.FirstOrDefault(t => t.Name == modelName);

                if (modelType != null)
                {
                    cfg.CreateMap(entityType, modelType);
                    cfg.CreateMap(modelType, entityType);
                }
            }
        });

        var mapper = config.CreateMapper();
        serviceCollection.AddSingleton(mapper);
        return serviceCollection;
    }
}

