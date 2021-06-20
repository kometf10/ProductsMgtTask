using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OA.Domain;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OA.Domin;
using OA.Domin.Logging;
using OA.Domin.Auditing;
using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.ProductsMgt.Products;
using OA.Domin.ProductsMgt.ProductCatigories;

namespace OA.DataAccess
{
    public partial class AppDbContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            httpContextAccessor = this.GetInfrastructure().GetRequiredService<IHttpContextAccessor>();
        }


        //Exception Log
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }


        //Auditing 
        public DbSet<Audit> Audits { get; set; }

        //ProductsMgt
        public DbSet<Catigory> Catigories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCatigory> ProductCatigories { get; set; }

    }
}
