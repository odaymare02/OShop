﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OShop.API.Models;

namespace OShop.API.Data
{
    public class ApplicationDbContext:DbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        { }
        public DbSet<Category> Categories { get; set; }
    }
}
