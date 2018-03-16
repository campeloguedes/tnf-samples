﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Tnf.Runtime.Session;

namespace Querying.Infra.Context.Migration
{
    public class OrderContextFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<OrderContext>();

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile($"appsettings.Development.json", false)
                                    .Build();

            builder.UseSqlServer(configuration.GetConnectionString(InfraConsts.ConnectionStringName));

            return new OrderContext(builder.Options, NullTnfSession.Instance);
        }
    }
}