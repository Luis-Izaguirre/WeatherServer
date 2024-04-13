using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CountryModel;

//Derived class, generic class in this case 
public partial class CountriesSilverContext : IdentityDbContext<WorldCitiesUser>
{
    public CountriesSilverContext()
    {
    }

    public CountriesSilverContext(DbContextOptions<CountriesSilverContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var config = builder.Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }
    //We put all tables into database by changing context, updating OnModelCreating and creating migrations.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<City>(entity =>
        {    
            entity.HasKey(c => c.CityId).HasName("PK_City");
            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Country");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK_Country");
            entity.Property(e => e.Iso2).IsFixedLength();
            entity.Property(e => e.Iso3).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
