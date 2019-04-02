using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class SageX3ExtendContext : DbContext
    {
        public SageX3ExtendContext(DbContextOptions<SageX3ExtendContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReceiptExtend>().ToTable("ReceiptExtend");
            modelBuilder.Entity<TaskStatusDetail>().ToTable("TaskStatusDetail");
            modelBuilder.Entity<TaskStatusMaster>().ToTable("TaskStatusMaster")
                 .HasIndex(b => b.WorkGroupCode).IsUnique();
            modelBuilder.Entity<PurchaseOrderHeader>().ToTable("PurchaseOrderHeader");
            modelBuilder.Entity<PurchaseExtend>().ToTable("PurchaseExtend")
                .HasIndex(b => new { b.PRNumber,b.PrSageHeaderId }).IsUnique();
            modelBuilder.Entity<PurchaseLineExtend>().ToTable("PurchaseLineExtend");
        }

        // Dbset
        public DbSet<ReceiptExtend> ReceiptExtends { get; set; }
        public DbSet<TaskStatusDetail> TaskStatusDetails { get; set; }
        public DbSet<TaskStatusMaster> TaskStatusMasters { get; set; }
        public DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public DbSet<PurchaseExtend> PurchaseExtends { get; set; }
        public DbSet<PurchaseLineExtend> PurchaseLineExtends { get; set; }

    }
}
