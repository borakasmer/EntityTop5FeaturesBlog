using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DAL.Entities.DbContext;

public partial class NorthwindContext : Microsoft.EntityFrameworkCore.DbContext
{
    public NorthwindContext()
    {
    }
    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options)
    {

    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            // "OrderDate" property'sinin varlığını kontrol et
            if (entityEntry.Metadata.GetProperties().Any(p => p.Name == "OrderDate"))
            {
                // "OrderDate" varsa, değerini güncelle
                entityEntry.Property("OrderDate").CurrentValue = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }


    public virtual DbSet<Actions> Actions { get; set; }

    public virtual DbSet<AlphabeticalListOfProducts> AlphabeticalListOfProducts { get; set; }

    public virtual DbSet<Categories> Categories { get; set; }

    public virtual DbSet<CategorySalesFor1997> CategorySalesFor1997 { get; set; }

    public virtual DbSet<CurrentProductList> CurrentProductList { get; set; }

    public virtual DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCity { get; set; }

    public virtual DbSet<CustomerDemographics> CustomerDemographics { get; set; }

    public virtual DbSet<Customers> Customers { get; set; }

    public virtual DbSet<Employees> Employees { get; set; }

    public virtual DbSet<Invoices> Invoices { get; set; }

    public virtual DbSet<Modules> Modules { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsExtended> OrderDetailsExtended { get; set; }

    public virtual DbSet<OrderSubtotals> OrderSubtotals { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<OrdersQry> OrdersQry { get; set; }

    public virtual DbSet<ProductSalesFor1997> ProductSalesFor1997 { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrice { get; set; }

    public virtual DbSet<ProductsByCategory> ProductsByCategory { get; set; }

    public virtual DbSet<QuarterlyOrders> QuarterlyOrders { get; set; }

    public virtual DbSet<Region> Region { get; set; }

    public virtual DbSet<RoleGroups> RoleGroups { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<SalesByCategory> SalesByCategory { get; set; }

    public virtual DbSet<SalesTotalsByAmount> SalesTotalsByAmount { get; set; }

    public virtual DbSet<Shippers> Shippers { get; set; }

    public virtual DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarter { get; set; }

    public virtual DbSet<SummaryOfSalesByYear> SummaryOfSalesByYear { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<Territories> Territories { get; set; }

    public virtual DbSet<UserAction> UserAction { get; set; }

    public virtual DbSet<UserRoles> UserRoles { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<VwCustomerProducts> VwCustomerProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        optionsBuilder.UseSqlServer("Data Source=192.168.50.173;Initial Catalog=Northwind;Persist Security Info=True;User ID=sa;Password=YourPassword;pooling=True;min pool size=0;max pool size=100;MultipleActiveResultSets=True;TrustServerCertificate=True;");
        optionsBuilder.AddInterceptors(new AddTimeCommandInterceptor());

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Orders>()
            .Property<DateTime>("OrderDate"); // Gorunmesini istemedigimiz ama arkada kullandigimiz kolon.    

        modelBuilder.Entity<Products>()
           .Property(p => p.CategoryName)
           .HasConversion<string>(); // Store enum as string in the database

        //modelBuilder.Entity<Users>()
        //   .HasQueryFilter(p => !p.IsDeleted);

        //Tüm entity türlerini dolaşır
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Entity'nin tipini al
            var clrType = entityType.ClrType;

            // IsDeleted adlı bir property olup olmadığını kontrol et
            var isDeletedProperty = clrType.GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);

            // Eğer IsDeleted property’si varsa, bu entity için global filtre uygula
            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
            {
                var parameter = Expression.Parameter(clrType, "e");
                var filter = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, isDeletedProperty.Name),
                        Expression.Constant(false)
                    ),
                    parameter
                );
                modelBuilder.Entity(clrType).HasQueryFilter(filter);
            }
        }

        modelBuilder.Entity<Actions>(entity =>
        {
            entity.Property(e => e.ActionName).HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.IdModuleNavigation).WithMany(p => p.Actions)
                .HasForeignKey(d => d.IdModule)
                .HasConstraintName("FK_Actions_Modules");
        });

        modelBuilder.Entity<AlphabeticalListOfProducts>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Alphabetical list of products");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UnitPrice).HasColumnType("money");
        });

        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.HasIndex(e => e.CategoryName, "CategoryName");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.Picture).HasColumnType("image");
        });

        modelBuilder.Entity<CategorySalesFor1997>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Category Sales for 1997");

            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.CategorySales).HasColumnType("money");
        });

        modelBuilder.Entity<CurrentProductList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Current Product List");

            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
        });

        modelBuilder.Entity<CustomerAndSuppliersByCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Customer and Suppliers by City");

            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.ContactName).HasMaxLength(30);
            entity.Property(e => e.Relationship)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CustomerDemographics>(entity =>
        {
            entity.HasKey(e => e.CustomerTypeId).IsClustered(false);

            entity.Property(e => e.CustomerTypeId)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CustomerTypeID");
            entity.Property(e => e.CustomerDesc).HasColumnType("ntext");
        });

        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.HasIndex(e => e.City, "City");

            entity.HasIndex(e => e.CompanyName, "CompanyName");

            entity.HasIndex(e => e.PostalCode, "PostalCode");

            entity.HasIndex(e => e.Region, "Region");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.ContactName).HasMaxLength(30);
            entity.Property(e => e.ContactTitle).HasMaxLength(30);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.Fax).HasMaxLength(24);
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Region).HasMaxLength(15);

            entity.HasMany(d => d.CustomerType).WithMany(p => p.Customer)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerCustomerDemo",
                    r => r.HasOne<CustomerDemographics>().WithMany()
                        .HasForeignKey("CustomerTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CustomerCustomerDemo"),
                    l => l.HasOne<Customers>().WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CustomerCustomerDemo_Customers"),
                    j =>
                    {
                        j.HasKey("CustomerId", "CustomerTypeId").IsClustered(false);
                        j.IndexerProperty<string>("CustomerId")
                            .HasMaxLength(5)
                            .IsFixedLength()
                            .HasColumnName("CustomerID");
                        j.IndexerProperty<string>("CustomerTypeId")
                            .HasMaxLength(10)
                            .IsFixedLength()
                            .HasColumnName("CustomerTypeID");
                    });
        });

        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.HasIndex(e => e.LastName, "LastName");

            entity.HasIndex(e => e.PostalCode, "PostalCode");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.Extension).HasMaxLength(4);
            entity.Property(e => e.FirstName).HasMaxLength(10);
            entity.Property(e => e.HireDate).HasColumnType("datetime");
            entity.Property(e => e.HomePhone).HasMaxLength(24);
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.Notes).HasColumnType("ntext");
            entity.Property(e => e.Photo).HasColumnType("image");
            entity.Property(e => e.PhotoPath).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Region).HasMaxLength(15);
            entity.Property(e => e.Title).HasMaxLength(30);
            entity.Property(e => e.TitleOfCourtesy).HasMaxLength(25);

            entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation)
                .HasForeignKey(d => d.ReportsTo)
                .HasConstraintName("FK_Employees_Employees");

            entity.HasMany(d => d.Territory).WithMany(p => p.Employee)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeTerritories",
                    r => r.HasOne<Territories>().WithMany()
                        .HasForeignKey("TerritoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeTerritories_Territories"),
                    l => l.HasOne<Employees>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeTerritories_Employees"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "TerritoryId").IsClustered(false);
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("EmployeeID");
                        j.IndexerProperty<string>("TerritoryId")
                            .HasMaxLength(20)
                            .HasColumnName("TerritoryID");
                    });
        });

        modelBuilder.Entity<Invoices>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Invoices");

            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
            entity.Property(e => e.CustomerName).HasMaxLength(40);
            entity.Property(e => e.ExtendedPrice).HasColumnType("money");
            entity.Property(e => e.Freight).HasColumnType("money");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.Region).HasMaxLength(15);
            entity.Property(e => e.RequiredDate).HasColumnType("datetime");
            entity.Property(e => e.Salesperson).HasMaxLength(31);
            entity.Property(e => e.ShipAddress).HasMaxLength(60);
            entity.Property(e => e.ShipCity).HasMaxLength(15);
            entity.Property(e => e.ShipCountry).HasMaxLength(15);
            entity.Property(e => e.ShipName).HasMaxLength(40);
            entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
            entity.Property(e => e.ShipRegion).HasMaxLength(15);
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
            entity.Property(e => e.ShipperName).HasMaxLength(40);
            entity.Property(e => e.UnitPrice).HasColumnType("money");
        });

        modelBuilder.Entity<Modules>(entity =>
        {
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModuleName).HasMaxLength(50);
        });

        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK_Order_Details");

            entity.ToTable("Order Details");

            entity.HasIndex(e => e.OrderId, "OrderID");

            entity.HasIndex(e => e.OrderId, "OrdersOrder_Details");

            entity.HasIndex(e => e.ProductId, "ProductID");

            entity.HasIndex(e => e.ProductId, "ProductsOrder_Details");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasDefaultValue((short)1);
            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Products");
        });

        modelBuilder.Entity<OrderDetailsExtended>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Order Details Extended");

            entity.Property(e => e.ExtendedPrice).HasColumnType("money");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.UnitPrice).HasColumnType("money");
        });

        modelBuilder.Entity<OrderSubtotals>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Order Subtotals");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Subtotal).HasColumnType("money");
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.HasIndex(e => e.CustomerId, "CustomerID");

            entity.HasIndex(e => e.CustomerId, "CustomersOrders");

            entity.HasIndex(e => e.EmployeeId, "EmployeeID");

            entity.HasIndex(e => e.EmployeeId, "EmployeesOrders");

            //entity.HasIndex(e => e.OrderDate, "OrderDate");

            entity.HasIndex(e => e.ShipPostalCode, "ShipPostalCode");

            entity.HasIndex(e => e.ShippedDate, "ShippedDate");

            entity.HasIndex(e => e.ShipVia, "ShippersOrders");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Freight)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            //entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.RequiredDate).HasColumnType("datetime");
            entity.Property(e => e.ShipAddress).HasMaxLength(60);
            entity.Property(e => e.ShipCity).HasMaxLength(15);
            entity.Property(e => e.ShipCountry).HasMaxLength(15);
            entity.Property(e => e.ShipName).HasMaxLength(40);
            entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
            entity.Property(e => e.ShipRegion).HasMaxLength(15);
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShipVia)
                .HasConstraintName("FK_Orders_Shippers");
        });

        modelBuilder.Entity<OrdersQry>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Orders Qry");

            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Freight).HasColumnType("money");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Region).HasMaxLength(15);
            entity.Property(e => e.RequiredDate).HasColumnType("datetime");
            entity.Property(e => e.ShipAddress).HasMaxLength(60);
            entity.Property(e => e.ShipCity).HasMaxLength(15);
            entity.Property(e => e.ShipCountry).HasMaxLength(15);
            entity.Property(e => e.ShipName).HasMaxLength(40);
            entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
            entity.Property(e => e.ShipRegion).HasMaxLength(15);
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductSalesFor1997>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Product Sales for 1997");

            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.ProductSales).HasColumnType("money");
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.HasIndex(e => e.CategoryId, "CategoriesProducts");

            entity.HasIndex(e => e.CategoryId, "CategoryID");

            entity.HasIndex(e => e.ProductName, "ProductName");

            entity.HasIndex(e => e.SupplierId, "SupplierID");

            entity.HasIndex(e => e.SupplierId, "SuppliersProducts");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
            entity.Property(e => e.ReorderLevel).HasDefaultValue((short)0);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UnitPrice)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.UnitsInStock).HasDefaultValue((short)0);
            entity.Property(e => e.UnitsOnOrder).HasDefaultValue((short)0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<ProductsAboveAveragePrice>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Products Above Average Price");

            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.UnitPrice).HasColumnType("money");
        });

        modelBuilder.Entity<ProductsByCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Products by Category");

            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
        });

        modelBuilder.Entity<QuarterlyOrders>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Quarterly Orders");

            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).IsClustered(false);

            entity.Property(e => e.RegionId)
                .ValueGeneratedNever()
                .HasColumnName("RegionID");
            entity.Property(e => e.RegionDescription)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<RoleGroups>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Group).WithMany(p => p.Roles)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Roles_RoleGroups");
        });

        modelBuilder.Entity<SalesByCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Sales by Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(15);
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.ProductSales).HasColumnType("money");
        });

        modelBuilder.Entity<SalesTotalsByAmount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Sales Totals by Amount");

            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SaleAmount).HasColumnType("money");
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Shippers>(entity =>
        {
            entity.HasKey(e => e.ShipperId);

            entity.Property(e => e.ShipperId).HasColumnName("ShipperID");
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.Phone).HasMaxLength(24);
        });

        modelBuilder.Entity<SummaryOfSalesByQuarter>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Summary of Sales by Quarter");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
            entity.Property(e => e.Subtotal).HasColumnType("money");
        });

        modelBuilder.Entity<SummaryOfSalesByYear>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Summary of Sales by Year");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
            entity.Property(e => e.Subtotal).HasColumnType("money");
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.HasKey(e => e.SupplierId);

            entity.HasIndex(e => e.CompanyName, "CompanyName");

            entity.HasIndex(e => e.PostalCode, "PostalCode");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(40);
            entity.Property(e => e.ContactName).HasMaxLength(30);
            entity.Property(e => e.ContactTitle).HasMaxLength(30);
            entity.Property(e => e.Country).HasMaxLength(15);
            entity.Property(e => e.Fax).HasMaxLength(24);
            entity.Property(e => e.HomePage).HasColumnType("ntext");
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Region).HasMaxLength(15);
        });

        modelBuilder.Entity<Territories>(entity =>
        {
            entity.HasKey(e => e.TerritoryId).IsClustered(false);

            entity.Property(e => e.TerritoryId)
                .HasMaxLength(20)
                .HasColumnName("TerritoryID");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TerritoryDescription)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Territories_Region");
        });

        modelBuilder.Entity<UserAction>(entity =>
        {
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.IdModuleNavigation).WithMany(p => p.UserAction)
                .HasForeignKey(d => d.IdModule)
                .HasConstraintName("FK_UserAction_Modules");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAction)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_UserAction_Users");
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleGroupId).HasColumnName("RoleGroupID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.RoleGroup).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleGroupId)
                .HasConstraintName("FK_UserRoles_RoleGroups");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gsm)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwCustomerProducts>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VwCustomerProducts");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("CustomerID");
            entity.Property(e => e.ProductName).HasMaxLength(40);
            entity.Property(e => e.ShipCountry).HasMaxLength(15);
            entity.Property(e => e.Total).HasColumnType("money");
            entity.Property(e => e.UnitPrice).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public class AddTimeCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        // Eğer komut bir SELECT sorgusuysa
        if (command.CommandText.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            // Mevcut SELECT cümlesine şimdiki zaman kolonu ekle
            command.CommandText = ModifyCommandText(command.CommandText);
            Debug.WriteLine($"Modified SQL Command: {command.CommandText}");
        }

        return base.ReaderExecuting(command, eventData, result);
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        // Eğer komut bir SELECT sorgusuysa
        if (command.CommandText.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            // Mevcut SELECT cümlesine şimdiki zaman kolonu ekle
            command.CommandText = ModifyCommandText(command.CommandText);
            Debug.WriteLine($"Modified SQL Command: {command.CommandText}");
        }

        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }


    private string ModifyCommandText(string commandText)
    {
        // İlk SELECT'in indeksini bul
        int selectIndex = commandText.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);

        if (selectIndex >= 0)
        {
            // Sütun listesinin sonunu bul
            int fromIndex = commandText.IndexOf("FROM", selectIndex, StringComparison.OrdinalIgnoreCase);
            if (fromIndex > 0)
            {
                // FROM'dan önce GETDATE() AS CurrentTime ekle
                commandText = commandText.Insert(fromIndex, ", GETDATE() AS CurrentTime ");
            }
        }

        return commandText; // Orijinal sorguyu döndür
    }
}






