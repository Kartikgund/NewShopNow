using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NewShopNowBL2.Models
{
    public partial class DPTContext : DbContext
    {
        public DPTContext()
            : base("name=DPTContext")
        {
        }

        public virtual DbSet<tblCustomer> tblCustomers { get; set; }
        public virtual DbSet<tblOTP> tblOTPs { get; set; }
        public virtual DbSet<tblStock> tblStocks { get; set; }
        public virtual DbSet<tblStore> tblStores { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }
        public virtual DbSet<tblTransactionItem> tblTransactionItems { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblLogError> tblLogErrors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblCustomer>()
                .HasMany(e => e.tblTransactions)
                .WithRequired(e => e.tblCustomer)
                .HasForeignKey(e => e.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblStock>()
                .HasMany(e => e.tblTransactionItems)
                .WithRequired(e => e.tblStock)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblRole>()
                .HasMany(e => e.tblUsers)
                .WithRequired(e => e.tblRole)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);
        }

        internal void BulkInsert(System.Data.DataTable dataTable)
        {
            throw new NotImplementedException();
        }
    }
}
