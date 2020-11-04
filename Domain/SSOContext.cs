using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class SSOContext : DbContext, IDataProtectionKeyContext
    {
        public SSOContext(DbContextOptions<SSOContext> option) : base(option) { }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}