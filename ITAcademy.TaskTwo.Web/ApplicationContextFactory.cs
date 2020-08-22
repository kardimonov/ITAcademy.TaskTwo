﻿using System.IO;
using ITAcademy.TaskTwo.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ITAcademy.TaskTwo.Web
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

            IConfigurationRoot builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = builder.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}