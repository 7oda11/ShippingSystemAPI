using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class ShippingContext: IdentityDbContext<ApplicationUser>
    {
        public ShippingContext()
        {
            
        }
        public ShippingContext(DbContextOptions<ShippingContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupPermission>()
                .HasKey(gp => new { gp.GroupId, gp.PermissionId }); 

            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPermissions)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Permission)
                .WithMany(p => p.GroupPermissions)
                .HasForeignKey(gp => gp.PermissionId);
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<DeliveryMan> DeliveryMen { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<EmployeeAssignOrderToDelivery> EmployeeAssignOrderToDeliveries { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<VendorPhones> VendorPhones { get; set; }
        public virtual DbSet<OrderCustomerPhones>  OrderCustomerPhones { get; set; }
        public virtual DbSet<Government> Governments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<WeightSetting> WeightSettings { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }








    }
}
